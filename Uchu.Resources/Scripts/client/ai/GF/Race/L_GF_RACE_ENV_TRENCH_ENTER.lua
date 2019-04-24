--------------------------------------------------------------
-- Script to change the fog settings in GF Racetrack as the player ENTERS Trench area
--
-- updated SeanB... 3/18/10
--------------------------------------------------------------

function onCollisionPhantom(self, msg)
if msg.objectID:GetID() == GAMEOBJ:GetControlledID():GetID() then
    --print ("You entered")
      
     LEVEL:SetLights(
		true, 0x535e6b,					--ambient color
		false, 0x000000,					--directional color
		false, 0xFFFFFF,					--specular color
		false, 0xFFFFFF,					--upper Hemi  color
		false, { 0.20, -0.58, 0.79 },	--directional direction
		true, 0x181b35,					--fog color

		true,                           --modifying draw distances (all of them)
        	00.0, 00.0,					--fog near min/max
		600.0, 600.0,					--fog far min/max
		160.0, 160.0,					--post fog solid min/max
		500.0, 1500.0,					--post fog fade min/max
		8000.0, 8000.0,	    			--static object cutoff min/max
		8000.0, 8000.0,	     			--dynamic object cutoff min/max

		true, "mesh\env\env_gen_sky_lightblue.nif",

		1.0					-- blend time
	 )	
	end		
end