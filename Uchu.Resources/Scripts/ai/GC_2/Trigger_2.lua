
require('o_mis')
require('State')


function onStartup(self)
   
    self:SetProximityRadius { radius = 25 , name = "HAX" }
    self:SetVar("skybox","mesh/env/challenge_sky_light_2awesome.nif")
	self:SetVar("skylayer","(invalid)")
	self:SetVar("ringlayer0","(invalid)")
	self:SetVar("ringlayer1","(invalid)")
	self:SetVar("ringlayer2","(invalid)")
	self:SetVar("ringlayer3","(invalid)")

end

function onProximityUpdate(self, msg)
   
    if self:GetVar("haxor") == nil then
     
       
        LEVEL:SetSkyDome (
		self:GetVar("skybox")
        )
	
	
	    LEVEL:SetLights(
		true,0x5E994F, --ambient color
		true,0xFFFFFF, --directional color
		true,0xFFFFFF, --specular color
		true,0xFFFFFF, --upper Hemi  color
		true,{550.0,-1990.0,550.0}, --directional direction
		true,0xBCEEFF,           --fog color

        true,                           --modifying draw distances (all of them)
        150.0, 150.0,					--fog near min/max
		850.0, 850.0,					--fog far min/max
		160.0, 160.0,					--post fog solid min/max
		500.0, 500.0,					--post fog fade min/max
		1510.0, 1510.0,	    			--static object cutoff min/max
		1010.0, 1010.0,	     			--dynamic object cutoff min/max

		false,"mesh/env/challenge_sky_light_2awesome.nif"
        )
               
    end
    
    

end


