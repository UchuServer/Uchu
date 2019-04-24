--------------------------------------------------------------
-- Script to change the fog settings in GF Racetrack as the player ENTERS Start area
--
-- updated SeanB... 3/18/10
--------------------------------------------------------------

function DoDisableCLUTSun(self)
    GAMEOBJ:GetTimer():CancelAllTimers(self )
    -- Reset our rendering back to untinted
    LEVEL:CLUTEffect( "(none)", 0.0, 1.0, 0.0, false )
end


function onCollisionPhantom(self, msg)
	if msg.objectID:GetID() == GAMEOBJ:GetControlledID():GetID() then
		--print ("You entered")
      
     		LEVEL:SetLights(
			true, 0xFFFFFF,					--ambient color
			false, 0x000000,				--directional color
			false, 0xFFFFFF,				--specular color
			false, 0xFFFFFF,				--upper Hemi  color
			false, { 0.65, -0.52, 0.55 },		--directional direction
			true, 0xb2dcff,					--fog color

			true,						--modifying draw distances (all of them)
        		200.0, 200.0,					--fog near min/max
			2000.0, 2000.0,					--fog far min/max
			160.0, 160.0,					--post fog solid min/max
			3500.0, 3500.0,					--post fog fade min/max
			8000.0, 8000.0,	    				--static object cutoff min/max
			8000.0, 8000.0,	     				--dynamic object cutoff min/max

			true, "mesh\env\env_sky_ra_gf.nif",

			1.0						-- blend time
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

