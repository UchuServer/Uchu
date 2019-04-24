--------------------------------------------------------------
-- Client-side script for a LUT to appear when a player is in a volume
-- 
-- created brandi... 11/5/10
--------------------------------------------------------------

-- ***********************************************************
-- put the dds file name of the LUT in the config data on the volume in happy flower
-- example    LUT   0:aura_mar_underground_LUT_gamefile.dds
-- ***********************************************************

-- We start our effect when we hit the collision phantom
function onCollisionPhantom(self, msg)
	local LUTfile = self:GetVar("LUT")
	if LUTfile then
		LEVEL:CLUTEffect( LUTfile, 0.2, 0.0, 1.0, false )
	else
		if self:GetVersioningInfo().bIsInternal then
			print("!!!! You don't have a LUT file attached to this volume through Happy Flower.!!!")
		end
	end
end

-- We disable our effect when we leave the collision phantom
function onOffCollisionPhantom(self, msg)
    LEVEL:CLUTEffect( "(none)", 0.0, 1.0, 0.0, false )
end