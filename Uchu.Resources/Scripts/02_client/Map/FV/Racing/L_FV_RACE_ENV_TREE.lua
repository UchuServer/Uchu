--------------------------------------------------------------
-- Script to change the fog settings in FV Racetrack as the player JUMPS INTO the TREE
-- Modified from Brandi's fog script(s) for GF
-- updated SteveY... 8-25-2010
--------------------------------------------------------------


function DoDisableCLUTSun(self)
    GAMEOBJ:GetTimer():CancelAllTimers(self )
    -- Reset our rendering back to untinted
    LEVEL:CLUTEffect( "(none)", 0.0, 1.0, 0.0, false )
end


function onCollisionPhantom(self, msg)
   if msg.objectID:GetID() == GAMEOBJ:GetControlledID():GetID() then
      
     LEVEL:SetSkyDome("mesh/env/env_fv_sky_mantis.nif")
     LEVEL:SetLights(
		true, 0x494140,					--ambient color
		false, 0xFFB26B,					--directional color
		false, 0xEEBB00,					--specular color
		false, 0x5A5A5A,					--upper Hemi  color
		false, { -0.44, -0.77, -0.46 },	--directional direction
		true, 0x465C76,					--fog color

		true,                           --modifying draw distances (all of them)
    	    	200.0, 400.0,					--fog near min/max
		900.0, 975.0,					--fog far min/max
		10000.0, 10000.0,					--post fog solid min/max
		100.0, 200.0,					--post fog fade min/max
		8000.0, 8000.0,	    			--static object cutoff min/max
		8000.0, 8000.0,	     			--dynamic object cutoff min/max

		true, "mesh/env/env_fv_sky_mantis.nif",

		1.0					-- blend time
	 )
	 
	-- Starting the fade on phantom collision
		GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "StartCLUTSun", self )
	end			
end


-- Timer controlling and calling the Sun Fade
function onTimerDone(self, msg)
    if (msg.name == "StartCLUTSun") then  
        LEVEL:CLUTEffect( "LUT_2xsunny.dds", 0.5, 0.0, 1.0, false)
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.5, "EndCLUTSun", self )
    end
    if (msg.name == "EndCLUTSun") then  
        LEVEL:CLUTEffect( "(none)", 4, 1.0, 0.0, false )
    end
end



-- We disable the effect if the script component is shut down for any reason
function onShutdown(self)
    DoDisableCLUTSun(self)
end
