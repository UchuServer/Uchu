--------------------------------------------------------------
-- Script to change the fog settings and CLUT in GF Racetrack as the player ENTERS Skull range
--
-- updated NateS... 3/24/10
--------------------------------------------------------------

function DoDisableCLUTSkull(self)
    GAMEOBJ:GetTimer():CancelAllTimers(self )
    -- Reset our rendering back to untinted / unCLUT'ed
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
			true, 0xff6a12,					--fog color

			true,						--modifying draw distances (all of them)
        		00.0, 00.0,					--fog near min/max
			800.0, 800.0,					--fog far min/max
			160.0, 160.0,					--post fog solid min/max
			3500.0, 3500.0,					--post fog fade min/max
			8000.0, 8000.0,	    				--static object cutoff min/max
			8000.0, 8000.0,	     				--dynamic object cutoff min/max

			true, "mesh\env\env_sky_ra_gf.nif",

			2.0						-- blend time
	)
	-- Starting the clut on phantom collision
		GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "StartCLUTSkull", self )
   	
	end	

end

-- Timer controlling and calling the CLUT
function onTimerDone(self, msg)
    if (msg.name == "StartCLUTSkull") then  
        LEVEL:CLUTEffect( "flamin_skull_lut_gamefile.dds", 4, 0.0, 1.0, false)
        GAMEOBJ:GetTimer():AddTimerWithCancel( 4, "EndCLUTSkull", self )
    end
    if (msg.name == "EndCLUTSkull") then  
        LEVEL:CLUTEffect( "(none)", 4, 1.0, 0.0, false )
    end
end


-- We disable the effect if the script component is shut down for any reason
function onShutdown(self)
    DoDisableCLUTSkull(self)
end





