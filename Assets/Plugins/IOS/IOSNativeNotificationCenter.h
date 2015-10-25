//
//  IOSNativeNotificationCenter.h
//  Unity-iPhone
//
//  Created by lacost on 11/9/13.
//
//

#import <Foundation/Foundation.h>
#import <GameKit/GameKit.h>

@interface IOSNativeNotificationCenter : NSObject


+ (IOSNativeNotificationCenter *)sharedInstance;
- (void) scheduleNotification: (int) time message: (NSString*) messgae sound: (bool *)sound badges: (int)badges;
- (void) showNotificationBanner: (NSString*) title message: (NSString*) message ;
- (void) cancelNotifications;
- (void) applicationIconBadgeNumber: (int)badges;

@end
