//
//  Ration.h
//  DFTUnityExtensionApp
//
//  Created by DFT JSC on 11/18/14.
//  Copyright (c) 2014 Vũ Ngọc Giang. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface Ration : NSObject

@property (nonatomic, strong) NSString *nid;
@property (nonatomic) NSInteger type;
@property (nonatomic, strong) NSString *nname;
@property (nonatomic) NSInteger kind;
@property (nonatomic) NSInteger weight;
@property (nonatomic) NSInteger priority;
@property (nonatomic, strong) NSString *key;

- (Ration *)initWithObject:(id)object;

@end
