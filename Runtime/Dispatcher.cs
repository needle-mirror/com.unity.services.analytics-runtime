using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity.Services.Analytics.Internal
{
    public class Dispatcher
    {
        readonly IBuffer m_DataBuffer;
        UnityWebRequestAsyncOperation m_Request;
        int m_RequestSentTokens;

        public string CollectUrl { get; set; }

        public Dispatcher(IBuffer buffer)
        {
            m_DataBuffer = buffer;
        }

        public void Flush()
        {
            // There is still a request pending.
            if (m_Request != null)
            {
                return;
            }

            FlushBufferToService();
        }

        void FlushBufferToService()
        {
            // Serialize it into a JSON Blob, then POST it to the Collect bulk URL.
            // 'Bulk Events' -> https://docs.deltadna.com/advanced-integration/rest-api/

            (string collectData, int sentTokens) = m_DataBuffer.Serialize();

            if (string.IsNullOrEmpty(collectData))
            {
                return;
            }

            byte[] postBytes = Encoding.UTF8.GetBytes(collectData);
            
            UnityWebRequest request = new UnityWebRequest(CollectUrl, UnityWebRequest.kHttpVerbPOST);
            UploadHandlerRaw upload = new UploadHandlerRaw(postBytes);
            upload.contentType = "application/json";
            request.uploadHandler = upload;

            m_RequestSentTokens = sentTokens;
            m_Request = request.SendWebRequest();
            m_Request.completed += UploadComplete;
            
            #if UNITY_ANALYTICS_EVENT_LOGS
            Debug.Log("Uploading events...");
            #endif
        }

        void UploadComplete(AsyncOperation _)
        {
            long code = m_Request.webRequest.responseCode;

            #if UNITY_ANALYTICS_EVENT_LOGS
            Debug.Assert(code == 204, "Incorrect response, check your JSON for errors.");
            #endif

            if (!m_Request.webRequest.isNetworkError && code == 204)
            {
                #if UNITY_ANALYTICS_EVENT_LOGS
                Debug.LogFormat("Events uploaded successfully!", code);
                #endif

                // Remove the sent tokens from the buffer because they were accepted
                m_DataBuffer.RemoveSentTokens(m_RequestSentTokens);

                // We have got the internet so the disk cache is no longer needed.
                // If the internet fails again, we will flush the buffer from scratch at that time.
                m_DataBuffer.ClearDiskCache();
            }
            else
            {
                #if UNITY_ANALYTICS_EVENT_LOGS
                if (m_Request.webRequest.isNetworkError)
                {
                    Debug.Log("Events failed to upload (network error) -- will retry at next heartbeat.");
                }
                else
                {
                    Debug.LogFormat("Events failed to upload (code {0}) -- will retry at next heartbeat.", code);
                }
                #endif
                m_DataBuffer.FlushToDisk();
            }

            // Clear the request to allow another request to be sent.
            m_Request.webRequest.Dispose();
            m_Request = null;
        }
    }
}
