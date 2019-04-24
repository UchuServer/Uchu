--------------------------------------------------------------
-- Script to change the fog settings in GF Racetrack as the player ENTERS Beach area
--
-- updated SeanB... 3/18/10
--------------------------------------------------------------

function onCollisionPhantom(self, msg)
   if msg.objectID:GetID() == GAMEOBJ:GetControlledID():GetID() then
      
     LEVEL:SetLights(
		true, 0xFFFFFF,					--ambient color
		false, 0xFFFFFF,					--directional color
		false, 0xFFFFFF,					--specular color
		false, 0xFFFFFF,					--upper Hemi  color
		false, { 0.17, -0.81, 0.56 },	--directional direction
		true, 0x84c9ff,					--fog color

		true,                           --modifying draw distances (all of them)
        	350.0, 350.0,					--fog near min/max
		2000.0, 2000.0,					--fog far min/max
		160.0, 160.0,					--post fog solid min/max
		1500.0, 2500.0,					--post fog fade min/max
		8000.0, 8000.0,	    			--static object cutoff min/max
		8000.0, 8000.0,	     			--dynamic object cutoff min/max

		true, "mesh\env\env_gen_sky_lightblue.nif",

		1.0					-- blend time
	 )
	end			
end