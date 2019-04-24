--------------------------------------------------------------
-- Client side for lightning puzzle in the lighting dungeon
-- 
-- created by brandi... 2/10/11
--------------------------------------------------------------

local fxName 	= "shake"
local fxID 		= 6030


function onStartup(self)
    debugPrint(self,"** This a Prototype Script attached to " .. self:GetName().name .. ". **")
    debugPrint(self,"** This script needs to be completed by Someone. **")
    debugPrint(self,"** This file is located at <res/scripts/02_client/map/LD>. **")
end

-- Timers control every aspect of the lightning; this is where the effect itself lives
function onNotifyClientObject(self, msg) 
	
	-- parse through the table of network vars that were updated
		if (msg.name == "StartCLUTLightning") then  
			LEVEL:CLUTEffect( "GF_LightningLUT.dds", 0.2, 0.0, 1.0, false )
		elseif (msg.name == "StartFlash") then
			LEVEL:FadeEffect( 1.0, 1.0, 1.0, 0.4, 1.0, 1.0, 1.0, 0.1, 0.1, false )
		elseif (msg.name == "EndFlash") then  
			LEVEL:CLUTEffect( "(none)", 0.1, 1.0, 0.0, false )
			LEVEL:FadeEffect( 1.0, 1.0, 1.0, 0.1, 1.0, 1.0, 1.0, 0.05, 0.1, false )

		elseif (msg.name == "EyeFlashAdjustment") then  
			LEVEL:FadeEffect( 1.0, 1.0, 1.0, 0.05, 0.0, 0.0, 0.0, 0.0, 0.5, false )

		elseif msg.name == "CamShake" then
			msg.paramObj:PlayFXEffect{effectID = fxID, effectType = fxName}
		end
	
end

-- print function that only works in an internal build
function debugPrint(self, text)	
	if self:GetVersioningInfo().bIsInternal then
		print(text)
	end
end