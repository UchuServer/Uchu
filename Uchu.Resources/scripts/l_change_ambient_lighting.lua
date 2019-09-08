require('o_mis')



function onCollisionPhantom(self, msg)
    --print ("You entered")
      
     LEVEL:SetLights(
		true, 0x476A72,					--ambient color
		false, 0xFFFFFF,					--directional color
		false, 0xFFFFFF,					--specular color
		false, 0xFFFFFF,					--upper Hemi  color
		false, { 1300.00, -1990.00, -2990.00 },	--directional direction
		true, 0x4FBCD3,					--fog color

		true,                           --modifying draw distances (all of them)
        80.0, 80.0,					--fog near min/max
		500.0, 298.0,					--fog far min/max
		175.0, 175.0,					--post fog solid min/max
		500.0, 500.0,					--post fog fade min/max
		8500.0, 8500.0,	    			--static object cutoff min/max
		8000.0, 8000.0,	     			--dynamic object cutoff min/max

		true, "Mesh\env\env_won_gnar_jungle_sky.nif"
	 )			
end


function onOffCollisionPhantom(self, msg)
    --print ("You exited")
   
     LEVEL:SetLights(
		true, 0x476a72,					--ambient color
		false, 0xFFFFFF,					--directional color
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

		true, "Mesh\env\env_sky_headspace_01.nif"
	 )			
end
