--------------------------------------------------------------
-- Script to change the fog settings in Forbidden Valley as the player moves back down the tree
--
-- updated Brandi... 2/8/10
-- updated abeechler... 2/10/11 - Fixed incorrect skybox paths
--------------------------------------------------------------

function onCollisionPhantom(self, msg)
    --print ("You exited")
	
	LEVEL:SetLights(
			true, 0x3D4D75,					--ambient color
			false, 0xFFFFFF,					--directional colorZ:\LWO\4_game\client\res\macros\gfstart.scm
			false, 0xFFFFFF,					--specular color
			false, 0x6B6B6B,					--upper Hemi  color
			false, { 0.84, -0.54, -0.08 },	--directional direction
			true, 0x0f2430,					--fog color

			true,                           --modifying draw distances (all of them)
			1.0, 100.0,					--fog near min/max
			550.0, 550.0,					--fog far min/max
			100.0, 600.0,					--post fog solid min/max
			100.0, 700.0,					--post fog fade min/max
			8000.0, 8000.0,	    			--static object cutoff min/max
			8000.0, 8000.0,	     			--dynamic object cutoff min/max

			false, "mesh/env/fv_tree_scene-skybox.nif",

			3.0					-- blend time
	)			
end
