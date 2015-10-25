//
//  Ration.m
//  DFTUnityExtensionApp
//
//  Created by DFT JSC on 11/18/14.
//  Copyright (c) 2014 Vũ Ngọc Giang. All rights reserved.
//

#import "Ration.h"

@implementation Ration

-(Ration *)initWithObject:(id)object
{
	self = [super init];
	if (self) {
		self.nid = [object objectForKey:@"nid"];
		self.type = [[object objectForKey:@"type"] integerValue];
		self.nname = [object objectForKey:@"nname"];
		self.kind = [[object objectForKey:@"kind"] integerValue];
		self.weight = [[object objectForKey:@"weight"] integerValue];
		self.priority = [[object objectForKey:@"priority"] integerValue];
		self.key = [object objectForKey:@"key"];
	}

	return self;
}

@end
