using Google.Apis.Upload;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using StreamingClient.Base.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using YouTubeLive.Base;

namespace YouTubeLive.VideoUploadSample.Console
{
    /// <summary>
    /// Video upload sample code from: https://developers.google.com/youtube/v3/code_samples/dotnet#upload_a_video
    /// </summary>

    public class Program
    {
        public static string clientID = "";     // SET YOUR OAUTH CLIENT ID
        public static string clientSecret = ""; // SET YOUR OAUTH CLIENT SECRET

        public static readonly List<OAuthClientScopeEnum> scopes = new List<OAuthClientScopeEnum>()
        {
            OAuthClientScopeEnum.ManageAccount,
            OAuthClientScopeEnum.ManageData,
            OAuthClientScopeEnum.ManagePartner,
            OAuthClientScopeEnum.ManagePartnerAudit,
            OAuthClientScopeEnum.ManageVideos,
            OAuthClientScopeEnum.ReadOnlyAccount,
            OAuthClientScopeEnum.ViewAnalytics,
            OAuthClientScopeEnum.ViewMonetaryAnalytics
        };

        public static void Main(string[] args)
        {
            Logger.LogOccurred += Logger_LogOccurred;
            Task.Run(async () =>
            {
                try
                {
                    if (string.IsNullOrEmpty(clientID) || string.IsNullOrEmpty(clientSecret))
                    {
                        throw new InvalidOperationException("Client ID and/or Client Secret are not set in the UnitTestBase class");
                    }

                    System.Console.WriteLine("Initializing connection");

                    YouTubeLiveConnection connection = await YouTubeLiveConnection.ConnectViaLocalhostOAuthBrowser(clientID, clientSecret, scopes);
                    if (connection != null)
                    {
                        Channel channel = await connection.Channels.GetMyChannel();
                        if (channel != null)
                        {
                            System.Console.WriteLine("Connection successful. Logged in as: " + channel.Snippet.Title);

                            System.Console.WriteLine("Performing video upload...");

                            var video = new Video();
                            video.Snippet = new VideoSnippet();
                            video.Snippet.Title = "Test Video - " + DateTimeOffset.Now.ToString("yyyy-MM-dd-HH-mm");
                            video.Snippet.Description = "Test Video Description";
                            video.Snippet.Tags = new string[] { "tag1", "tag2" };
                            video.Snippet.CategoryId = "22";
                            video.Status = new VideoStatus();
                            video.Status.PrivacyStatus = "unlisted"; // or "private" or "public"

                            bool uploadFailureOccurred = false;
                            do
                            {
                                using (var fileStream = new FileStream("video.mp4", FileMode.Open))
                                {
                                    var videosInsertRequest = connection.GoogleYouTubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");

                                    videosInsertRequest.ProgressChanged += (IUploadProgress progress) =>
                                    {
                                        switch (progress.Status)
                                        {
                                            case UploadStatus.Uploading:
                                                System.Console.WriteLine("{0} bytes sent.", progress.BytesSent);
                                                break;

                                            case UploadStatus.Failed:
                                                uploadFailureOccurred = true;
                                                System.Console.WriteLine("An error prevented the upload from completing.\n{0}", progress.Exception);
                                                break;
                                        }
                                    };

                                    videosInsertRequest.ResponseReceived += (Video uploadedVideo) =>
                                    {
                                        video = uploadedVideo;
                                        System.Console.WriteLine("Video id '{0}' was successfully uploaded.", uploadedVideo.Id);
                                    };

                                    if (uploadFailureOccurred)
                                    {
                                        uploadFailureOccurred = false;
                                        await videosInsertRequest.ResumeAsync();
                                    }
                                    else
                                    {
                                        await videosInsertRequest.UploadAsync();
                                    }
                                }
                            } while (uploadFailureOccurred);

                            using (var fileStream = new FileStream("thumbnail.jpg", FileMode.Open))
                            {
                                ThumbnailsResource.SetMediaUpload thumbnailSetRequest = connection.GoogleYouTubeService.Thumbnails.Set(video.Id, fileStream, "image/jpeg");

                                thumbnailSetRequest.ProgressChanged += (IUploadProgress progress) =>
                                {
                                    switch (progress.Status)
                                    {
                                        case UploadStatus.Uploading:
                                            System.Console.WriteLine("{0} bytes sent.", progress.BytesSent);
                                            break;

                                        case UploadStatus.Failed:
                                            System.Console.WriteLine("An error prevented the upload from completing.\n{0}", progress.Exception);
                                            break;
                                    }
                                };

                                thumbnailSetRequest.ResponseReceived += (ThumbnailSetResponse uploadedThumbnail) =>
                                {
                                    if (uploadedThumbnail.Items.Count > 0)
                                    {
                                        System.Console.WriteLine("Thumbnail was successfully uploaded.");
                                    }
                                };

                                await thumbnailSetRequest.UploadAsync();
                            }

                            System.Console.WriteLine("Uploads completed!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.ToString());
                }
            });

            System.Console.ReadLine();
        }

        private static void Logger_LogOccurred(object sender, Log log)
        {
            if (log.Level >= LogLevel.Error)
            {
                System.Console.WriteLine(log.Message);
            }
        }
    }
}
