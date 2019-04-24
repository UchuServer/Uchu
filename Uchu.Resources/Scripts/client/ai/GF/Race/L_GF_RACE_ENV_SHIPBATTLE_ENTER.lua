--------------------------------------------------------------
-- Script to change the fog settings in GF Racetrack as the player ENTERS Beach area
--
-- updated SeanB... 3/18/10
--------------------------------------------------------------

function onCollisionPhantom(self, msg)
   if msg.objectID:GetID() == GAMEOBJ:GetControlledID():GetID() then

     LEVEL:SetSkyDome("mesh/env/env_sky_ra_gf.nif")     
     LEVEL:SetLights(
		true, 0x315596,					--ambient color
		false, 0xFFFFFF,					--directional color
		false, 0xFFFFFF,					--specular color
		false, 0xFFFFFF,					--upper Hemi  color
		false, { 0.18, -0.52, 0.83 },	--directional direction
		true, 0x122d4c,					--fog color

		true,                           --modifying draw distances (all of them)
    	    	00.0, 00.0,					--fog near min/max
		1000.0, 800.0,					--fog far min/max
		160.0, 160.0,					--post fog solid min/max
		3500.0, 3500.0,					--post fog fade min/max
		8000.0, 8000.0,	    			--static object cutoff min/max
		8000.0, 8000.0,	     			--dynamic object cutoff min/max

		true, "mesh\env\env_sky_ra_gf.nif",

		3.0					-- blend time
	 )
	end			
end