//
//  IOSNativeNotificationCenter.m
//  Unity-iPhone
//
//  Created by lacost on 11/9/13.
//
//

#import "ISNDataConvertor.h"
#import "IOSNativeNotificationCenter.h"

@implementation IOSNativeNotificationCenter


static IOSNativeNotificationCenter *sharedHelper = nil;

+ (IOSNativeNotificationCenter *) sharedInstance {
    if (!sharedHelper) {
        sharedHelper = [[IOSNativeNotificationCenter alloc] init];
        
    }
    return sharedHelper;
}

-(void) scheduleNotification:(int)time message:(NSString *)messgae sound:(bool *)sound badges: (int)badges {
    
    
    UILocalNotification* localNotification = [[UILocalNotification alloc] init];
    localNotification.fireDate = [NSDate dateWithTimeIntervalSinceNow:time];
    localNotification.alertBody = messgae;
    localNotification.timeZone = [NSTimeZone defaultTimeZone];
    if (badges > 0)
        localNotification.applicationIconBadgeNumber = badges;
    
    
    if(sound) {
        localNotification.soundName = UILocalNotificationDefaultSoundName;
    }
    
    [[UIApplication sharedApplication] scheduleLocalNotification:localNotification];
    
}



-(void) showNotificationBanner:(NSString *)title message:(NSString *)message {
    [GKNotificationBanner showBannerWithTitle:title message:message completionHandler:^{}];
}

- (void) cancelNotifications {
    [[UIApplication sharedApplication] cancelAllLocalNotifications];
}

- (void) applicationIconBadgeNumber:(int) badges {
    [UIApplication sharedApplication].applicationIconBadgeNumber = badges;
}


@end

extern "C" {
    
    void _cancelNotifications() {
        [[IOSNativeNotificationCenter sharedInstance] cancelNotifications];
    }
    
    
    void _scheduleNotification (int time, char* messgae, bool* sound, int badges)  {
        [[IOSNativeNotificationCenter sharedInstance] scheduleNotification:time message:[ISNDataConvertor charToNSString:messgae] sound:sound badges:badges];
    }
    
    void _showNotificationBanner (char* title, char* messgae)  {
        [[IOSNativeNotificationCenter sharedInstance] showNotificationBanner:[ISNDataConvertor charToNSString:title] message:[ISNDataConvertor charToNSString:messgae]];
    }
    void _applicationIconBadgeNumber (int badges) {
        [[IOSNativeNotificationCenter sharedInstance] applicationIconBadgeNumber:badges];
    }
}
