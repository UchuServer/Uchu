
--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_AvantGardens')





--------------------------------------------------------------
-- When objects are loaded via zone notification
--------------------------------------------------------------
function onObjectLoadedStagePlatforms( self, msg )

      
	-- also look for the concert_stage and pass these ID's on to it
      		
	if ( msg.templateID == CONSTANTS["LOT_STAGE_PLATFORM_LARGE"] ) then
		
		storeObjectByName( self, "LargeStagePlatform", msg.objectID )
		SendPlatformIDsToStage( self )

		
	elseif ( msg.templateID == CONSTANTS["LOT_STAGE_PLATFORM_SMALL"] ) then
		
		storeObjectByName( self, "SmallStagePlatform", msg.objectID )
		SendPlatformIDsToStage( self )
		
		
	elseif ( msg.templateID == CONSTANTS["LOT_STAGE"] ) then
		
		storeObjectByName( self, "Stage", msg.objectID )
		SendPlatformIDsToStage( self )
		
	
	end
		
end





--------------------------------------------------------------
-- if the stage is loaded and so are the 2 stage platforms,
-- send the ID's of the platforms to the stage
-- and the ID of the stage to the platforms
--------------------------------------------------------------
function SendPlatformIDsToStage( self )
	
	local stage = getObjectByName( self, "Stage" )
	local largePlatform = getObjectByName( self, "LargeStagePlatform" )
	local smallPlatform = getObjectByName( self, "SmallStagePlatform" )

	
	if ( stage ~= nil and
		largePlatform ~= nil and
		smallPlatform ~= nil ) then

		stage:NotifyObject{ name = "StoreLargePlatform", ObjIDSender = largePlatform }
		stage:NotifyObject{ name = "StoreSmallPlatform", ObjIDSender = smallPlatform }
		
		largePlatform:NotifyObject{ name = "StoreStage", ObjIDSender = stage }
		smallPlatform:NotifyObject{ name = "StoreStage", ObjIDSender = stage }
		
	end
		
		
end







