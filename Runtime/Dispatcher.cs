using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity.Services.Analytics.Internal
{
    public class Dispatcher
    {
        readonly Buffer m_DataBuffer;
        UnityWebRequestAsyncOperation m_Request;

        public string CollectUrl { get; set; }

        public Dispatcher(Buffer buffer)
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

            if (HasConnectivity())
            {
                FlushBufferToService();

                // We have got the internet back so the disk cache is no longer needed.
                // If the internet fails again, we will flush the buffer from scratch at that time.
                m_DataBuffer.ClearDiskCache();
            }
            else
            {
                m_DataBuffer.FlushToDisk();
            }
        }

        void FlushBufferToService()
        {
            // Serialize it into a JSON Blob, then POST it to the Collect bulk URL.
            // 'Bulk Events' -> https://docs.deltadna.com/advanced-integration/rest-api/

            string collectData = m_DataBuffer.Serialize();

            if (string.IsNullOrEmpty(collectData))
            {
                return;
            }

            byte[] postBytes = Encoding.UTF8.GetBytes(collectData);
            
            UnityWebRequest request = new UnityWebRequest(CollectUrl, UnityWebRequest.kHttpVerbPOST);
            UploadHandlerRaw upload = new UploadHandlerRaw(postBytes);
            upload.contentType = "application/json";
            request.uploadHandler = upload;
            
            m_Request = request.SendWebRequest();
            
            #if UNITY_ANALYTICS_DEVELOPMENT
            Debug.Log("<color=#00ffff>Sent Data</color>");
            #endif
            
            m_Request.completed += _ =>
            {
                #if UNITY_ANALYTICS_DEVELOPMENT
                long code = m_Request.webRequest.responseCode;
                Debug.Assert(code == 204, "Incorrect response, check your JSON for errors.");
                Debug.LogFormat("<color=#00ffff>Collect result code - {0}</color>", code);
                #endif
                
                // Clear the request to allow another request to be sent.
                m_Request = null;
            };
        }

        static bool HasConnectivity()
        {
            // TODO: LOSDK-413 as this property isn't the most reliable way of determining connectivity
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
    }
}
