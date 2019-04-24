--------------------------------------------------------------
-- Script to change the fog settings in FV Racetrack as the player JUMPS OFF the starting bridge
-- Modified from Brandi's fog script(s) for GF
-- updated SteveY... 8-25-2010
--------------------------------------------------------------

function onCollisionPhantom(self, msg)
   if msg.objectID:GetID() == GAMEOBJ:GetControlledID():GetID() then
      
     LEVEL:SetSkyDome("mesh/env/env_fv_sky_mantis.nif")
     LEVEL:SetLights(
		true, 0x494140,					--ambient color
		false, 0xFFB26B,					--directional color
		false, 0xEEBB00,					--specular color
		false, 0x5A5A5A,					--upper Hemi  color
		false, { -0.44, -0.77, -0.46 },	--directional direction
		true, 0x170F21,					--fog color

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
	end			
end