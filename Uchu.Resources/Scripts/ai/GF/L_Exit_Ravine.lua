--------------------------------------------------------------
-- updated abeechler... 2/10/11 - Fixed incorrect skybox paths
--------------------------------------------------------------

function onCollisionPhantom(self, msg)
    --print ("You exited")
      
     LEVEL:SetLights(
			true, 0x476a72,					--ambient color
			false, 0xFFFFFF,					--directional colorZ:\LWO\4_game\client\res\macros\gfstart.scm
			false, 0xFFFFFF,					--specular color
			false, 0xFFFFFF,					--upper Hemi  color
			false, { 1300.00, -1990.00, -2990.00 },	--directional direction
			true, 0x3a7599,					--fog color

			true,                           --modifying draw distances (all of them)
			50.0, 50.0,					--fog near min/max
			500.0, 174.0,					--fog far min/max
			160.0, 160.0,					--post fog solid min/max
			500.0, 500.0,					--post fog fade min/max
			8000.0, 8000.0,	    			--static object cutoff min/max
			8000.0, 8000.0,	     			--dynamic object cutoff min/max

			false, "Mesh/env/env_sky_headspace_01.nif",
			3.0
	 )			
end