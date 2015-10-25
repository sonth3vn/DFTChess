//
//  ISNCamera.m
//  Unity-iPhone
//
//  Created by Osipov Stanislav on 6/10/14.
//
//

#import "ISNCamera.h"

@implementation ISNCamera

static ISNCamera *_sharedInstance;


+ (id)sharedInstance {
    
    if (_sharedInstance == nil)  {
        _sharedInstance = [[self alloc] init];
    }
    
    return _sharedInstance;
}

- (void) saveToCameraRoll:(NSString *)media {
    NSData *imageData = [[NSData alloc] initWithBase64Encoding:media];
    UIImage *image = [[UIImage alloc] initWithData:imageData];
    
    UIImageWriteToSavedPhotosAlbum(image, nil, nil, nil);

}

-(void) GetImageFromAlbum {
   // UIi,a
    
    [self GetImage:UIImagePickerControllerSourceTypeSavedPhotosAlbum];
}

-(void) GetImageFromCamera {
    [self GetImage:UIImagePickerControllerSourceTypeCamera];
}



-(void) GetImage: (UIImagePickerControllerSourceType )source {
    UIViewController *vc =  UnityGetGLViewController();
    
    UIImagePickerController * picker = [[UIImagePickerController alloc] init];
	picker.delegate = self;
	
    picker.sourceType = source;
    
    

    
	[vc presentModalViewController:picker animated:YES];
    
   

}


-(void) imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary *)info {
    UIViewController *vc =  UnityGetGLViewController();
    [vc dismissModalViewControllerAnimated:YES];
    
    UIImage *photo = [info objectForKey:UIImagePickerControllerOriginalImage];
    
    NSString *encodedImage = @"";
    if (photo == nil) {
         NSLog(@"no photo");
    } else {
        
        if(photo.size.width > 1024) {
           CGSize s = CGSizeMake(1024, 1024);
            
            CGFloat new_height = 1024 / (photo.size.width / photo.size.height);
            s.height = new_height;

            
         
            photo =   [ISNCamera imageWithImage:photo scaledToSize:s];
            
        }
        
        
        
        NSData *imageData = UIImagePNGRepresentation(photo);
        encodedImage = [imageData base64Encoding];
    }
    
   
    
    
   UnitySendMessage("IOSCamera", "OnImagePickedEvent", [ISNDataConvertor NSStringToChar:encodedImage]);
    
    

}

+ (UIImage *)imageWithImage:(UIImage *)image scaledToSize:(CGSize)newSize {
    //UIGraphicsBeginImageContext(newSize);
    // In next line, pass 0.0 to use the current device's pixel scaling factor (and thus account for Retina resolution).
    // Pass 1.0 to force exact pixel size.
    UIGraphicsBeginImageContextWithOptions(newSize, NO, 0.0);
    [image drawInRect:CGRectMake(0, 0, newSize.width, newSize.height)];
    UIImage *newImage = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    return newImage;
}

-(void) imagePickerControllerDidCancel:(UIImagePickerController *)picker {
    UIViewController *vc =  UnityGetGLViewController();
    [vc dismissModalViewControllerAnimated:YES];
    
    UnitySendMessage("IOSCamera", "OnImagePickedEvent", [ISNDataConvertor NSStringToChar:@""]);
}

extern "C" {
    
    
    //--------------------------------------
	//  IOS Native Plugin Section
	//--------------------------------------
    
    
    void _ISN_SaveToCameraRoll(char* encodedMedia) {
        NSString *media = [ISNDataConvertor charToNSString:encodedMedia];
        [[ISNCamera sharedInstance] saveToCameraRoll:media];
    }
    
    void _ISN_GetImageFromCamera() {
        [[ISNCamera sharedInstance] GetImageFromCamera];
    }
    
    
    
    void _ISN_GetImageFromAlbum() {
        [[ISNCamera sharedInstance] GetImageFromAlbum];
    }

}


@end
