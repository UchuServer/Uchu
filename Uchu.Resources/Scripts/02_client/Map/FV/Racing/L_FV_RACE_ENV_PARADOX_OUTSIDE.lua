--------------------------------------------------------------
-- Script to change the fog settings in FV Racetrack as the player JUMPS OFF the starting bridge
-- Modified from Brandi's fog script(s) for GF
-- updated SteveY... 8-25-2010
--------------------------------------------------------------

function onCollisionPhantom(self, msg)
   if msg.objectID:GetID() == GAMEOBJ:GetControlledID():GetID() then
      
     LEVEL:SetSkyDome("mesh/env/env_fv_sky_mantis.nif")
     LEVEL:SetLights(
		true, 0x334C4F,					--ambient color
		false, 0x8E7400,					--directional color
		false, 0xFA7300,					--specular color
		false, 0x5A5A5A,					--upper Hemi  color
		false, { -0.27, 0.96, 0.02 },	--directional direction
		true, 0x324551,					--fog color

		true,                           --modifying draw distances (all of them)
    	    	200.0, 300.0,					--fog near min/max
		900.0, 950.0,					--fog far min/max
		10000.0, 10000.0,					--post fog solid min/max
		100.0, 200.0,					--post fog fade min/max
		8000.0, 8000.0,	    			--static object cutoff min/max
		8000.0, 8000.0,	     			--dynamic object cutoff min/max

		true, "mesh/env/env_fv_sky_mantis.nif",

		1.0					-- blend time
	 )
	end			
end