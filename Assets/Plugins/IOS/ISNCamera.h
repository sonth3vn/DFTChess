//
//  ISNCamera.h
//  Unity-iPhone
//
//  Created by Osipov Stanislav on 6/10/14.
//
//

#import <Foundation/Foundation.h>
#include "ISNDataConvertor.h"
#if UNITY_VERSION < 450
#include "iPhone_View.h"
#endif

@interface ISNCamera : NSObject<UIImagePickerControllerDelegate, UINavigationControllerDelegate>

+ (id)   sharedInstance;
- (void) saveToCameraRoll:(NSString*)media;

-(void) GetImageFromAlbum;
-(void) GetImageFromCamera;

@end
