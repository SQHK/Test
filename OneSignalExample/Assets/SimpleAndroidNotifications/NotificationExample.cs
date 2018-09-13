using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;

namespace Assets.SimpleAndroidNotifications
{
    public class NotificationExample : MonoBehaviour
    {
        int count = 0;
        void Start() {
            Debug.Log("NotificationExample: start");
            #if UNITY_IOS
                NotificationServices.RegisterForNotifications(
                    NotificationType.Alert |
                    NotificationType.Badge |
                    NotificationType.Sound);
                StartCoroutine(CleanNotification());
            #endif
        }
        public void Rate()
        {
            Application.Quit();
            // Application.OpenURL("http://u3d.as/y6r");
        }

        public void OpenWiki()
        {
            Application.OpenURL("https://github.com/hippogamesunity/SimpleAndroidNotificationsPublic/wiki");
        }

        public void ScheduleSimple()
        {
            #if UNITY_ANDROID
            NotificationManager.Send(TimeSpan.FromSeconds(15), "Simple 15 notification", "Customize icon and color", new Color(1, 0.3f, 0.15f));
            #elif UNITY_IOS
            if(count == 0) {
               ScheduleNotificationForiOSWithMessage("Test ios notification", DateTime.Now.AddSeconds(5.0f)); 
               count++;
            } else if(count == 1) {
                ScheduleNotificationForiOSWithMessage("Test ios notification", DateTime.Now.AddSeconds(15.0f));
                count++;
            } else if(count == 2) {
                ScheduleNotificationForiOSWithMessage("Test ios notification", DateTime.Now.AddSeconds(25.0f));
            }
            #endif
        }

        public void ScheduleNormal()
        {
            NotificationManager.SendWithAppIcon(TimeSpan.FromSeconds(5), "Notification", "Notification with app icon", new Color(0, 0.6f, 1), NotificationIcon.Message);
        }

        void OnApplicationFocus(bool focus) {
            // Debug.Log("OnApplicationQuit: " + focus);
            // if(!focus)
            //     NotificationManager.Send(TimeSpan.FromSeconds(5), "Simple notification", "Customize icon and color", new Color(1, 0.3f, 0.15f));
            if(focus) {
                StartCoroutine(CleanNotification());
            }
        }

        public void ScheduleCustom()
        {
            var notificationParams = new NotificationParams
            {
                Id = UnityEngine.Random.Range(0, int.MaxValue),
                Delay = TimeSpan.FromSeconds(5),
                Title = "Custom notification",
                Message = "Message",
                Ticker = "Ticker",
                Sound = true,
                Vibrate = true,
                Light = true,
                SmallIcon = NotificationIcon.Heart,
                SmallIconColor = new Color(0, 0.5f, 0),
                LargeIcon = "app_icon"
            };

            NotificationManager.SendCustom(notificationParams);
        }

        public void CancelAll()
        {
            #if UNITY_ANDROID
            NotificationManager.CancelAll();
            #elif UNITY_IOS
            CancelSceuuleIOSNotification();
            #endif
        }

        void ScheduleNotificationForiOSWithMessage (string text, System.DateTime fireDate)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer) {
                Debug.Log("remoteNotificationCount: " + UnityEngine.iOS.NotificationServices.remoteNotificationCount);
                Debug.Log("localNotificationCount: " + UnityEngine.iOS.NotificationServices.localNotificationCount);
                Debug.Log("scheduledLocalNotifications: " + UnityEngine.iOS.NotificationServices.scheduledLocalNotifications.Length);
                UnityEngine.iOS.LocalNotification notification = new UnityEngine.iOS.LocalNotification ();
                notification.fireDate = fireDate;
                notification.alertAction = "Alert";
                notification.alertBody = text;
                notification.hasAction = false;
                notification.applicationIconBadgeNumber = UnityEngine.iOS.NotificationServices.scheduledLocalNotifications.Length + 1;
                notification.userInfo.Add("id", UnityEngine.Random.Range(0, int.MaxValue));
                UnityEngine.iOS.NotificationServices.ScheduleLocalNotification (notification);
            }        
        }
        
        void CancelSceuuleIOSNotification() {
            UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
        }

        IEnumerator CleanNotification(){
            #if UNITY_IPHONE
            UnityEngine.iOS.LocalNotification l = new UnityEngine.iOS.LocalNotification();
            l.applicationIconBadgeNumber = -1;
            l.hasAction = false;
            NotificationServices.PresentLocalNotificationNow(l);
            yield return new WaitForSeconds(0.2f);
            RearrageBradeNum();
            NotificationServices.ClearLocalNotifications();
            #endif
        }

        void RearrageBradeNum() {
            int notificationCount = UnityEngine.iOS.NotificationServices.scheduledLocalNotifications.Length;
            List<UnityEngine.iOS.LocalNotification>notifications = new List<UnityEngine.iOS.LocalNotification>();
        
            for(int i = 0; i < notificationCount; i++) {
                var notification = UnityEngine.iOS.NotificationServices.scheduledLocalNotifications[i];
                notification.applicationIconBadgeNumber = i + 1;
                notifications.Add(notification);
            }
            NotificationServices.CancelAllLocalNotifications();

            for(int i = 0; i < notifications.Count; i++) {
                UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notifications[i]);
            }
        }
    }
}