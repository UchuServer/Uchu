--------------------------------------------------------------
-- Script to change the fog settings in GF Racetrack as the player ENTERS Start area
--
-- updated SeanB... 3/18/10
--------------------------------------------------------------

function onCollisionPhantom(self, msg)
	if msg.objectID:GetID() == GAMEOBJ:GetControlledID():GetID() then
		--print ("You entered")
      
     		LEVEL:SetLights(
			true, 0x312b26,					--ambient color
			false, 0x000000,				--directional color
			false, 0xFFFFFF,				--specular color
			false, 0xFFFFFF,				--upper Hemi  color
			false, { 0.65, -0.52, 0.55 },			--directional direction
			true, 0x7a6556,					--fog color

			true,						--modifying draw distances (all of them)
        		00.0, 00.0,					--fog near min/max
			700.0, 700.0,					--fog far min/max
			160.0, 160.0,					--post fog solid min/max
			3500.0, 3500.0,					--post fog fade min/max
			8000.0, 8000.0,	    				--static object cutoff min/max
			8000.0, 8000.0,	     				--dynamic object cutoff min/max

			true, "mesh\env\env_sky_ra_gf.nif",

			1.0						-- blend time
	 )
	end			
end