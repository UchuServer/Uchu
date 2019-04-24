--------------------------------------------------------------
-- Script to change the fog settings in FV Racetrack as the player JUMPS OFF the starting bridge
-- Modified from Brandi's fog script(s) for GF
-- updated SteveY... 8-25-2010
--------------------------------------------------------------

function onCollisionPhantom(self, msg)
   if msg.objectID:GetID() == GAMEOBJ:GetControlledID():GetID() then
      
     LEVEL:SetSkyDome("mesh/env/env_sky_ra_fv_start.nif")
     LEVEL:SetLights(
		true, 0x916E6E,					--ambient color
		false, 0xD4E2EE,					--directional color
		false, 0xFFFFFF,					--specular color
		false, 0x6B6B6B,					--upper Hemi  color
		false, { 0.84, -0.54, -0.55 },	--directional direction
		true, 0x284858,					--fog color

		true,                           --modifying draw distances (all of them)
    	    	2.0, 2.0,					--fog near min/max
		600.0, 600.0,					--fog far min/max
		10000.0, 10000.0,					--post fog solid min/max
		100.0, 100.0,					--post fog fade min/max
		8000.0, 8000.0,	    			--static object cutoff min/max
		8000.0, 8000.0,	     			--dynamic object cutoff min/max

		true, "mesh/env/env_sky_ra_fv_start.nif",

		1.0					-- blend time
	 )
	end			
end
