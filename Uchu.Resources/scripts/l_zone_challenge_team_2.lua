require('o_mis')
require('State')

function onStartup(self)
    self:SetVar("Mission_A_Brick", 0 ) 
    self:SetVar("Mission_A_State", "idle") 
 
    self:SetVar("Mission_B_Brick", 0 ) 
    self:SetVar("Mission_B_State", "idle") 
   
    self:SetVar("Trigger_A_ID", 0) 
    self:SetVar("Trigger_B_ID", 0) 
    
    self:SetVar("MissionDone",false)
    
    brick_a_count = 0 
    mission_a_state = "idle"
   
    self:SetVar("skybox","mesh/env/challenge_sky_light_2awesome.nif")
	self:SetVar("skylayer","(invalid)")
	self:SetVar("ringlayer0","(invalid)")
	self:SetVar("ringlayer1","(invalid)")
	self:SetVar("ringlayer2","(invalid)")
	self:SetVar("ringlayer3","(invalid)")

  

   
    --GAMEOBJ:GetZoneControlID()
    
    self:UseStateMachine{} 
    
    Idle = State.create()
    Idle.onEnter = function(self)
     
    end
    Idle.onArrived = function(self)

    end    
    Stage1 = State.create()
    Stage1.onEnter = function(self)
        if self:GetVar("Mission_A_Brick") == 15 then
            self:SetVar("Mission_A_State","ADONE")
            
        end
   
    end 
    Stage1.onArrived = function(self)

    end   
     
    Stage2 = State.create()
    Stage2.onEnter = function(self)
   
    end   
    Stage2.onArrived = function(self)

    end    
    Stage3 = State.create()
    Stage3.onEnter = function(self)
   
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
		999.0, 999.0,					--fog far min/max
		160.0, 160.0,					--post fog solid min/max
		500.0, 500.0,					--post fog fade min/max
		1695.0, 1695.0,	    			--static object cutoff min/max
		1195.0, 1195.0,	     			--dynamic object cutoff min/max

		false,"mesh/env/challenge_sky_light_2awesome.nif"
	)


    end    
    Stage3.onArrived = function(self)

    end 
   
    addState(Stage1, "Stage1", "Stage1", self)
    addState(Stage2, "Stage2", "Stage2", self)
    addState(Stage3, "Stage3", "Stage3", self)
    addState(Idle, "Idle", "Idle", self)
    beginStateMachine("Idle", self) 


    
    
end



--------------------------------------------------------------

-- Sent from a player when responding from a messagebox

--------------------------------------------------------------

function onMessageBoxRespond(self, msg)
 local t = msg
 msg = msg
end

function getA(self)
    targetID = self:GetVar("Trigger_A_ID")
    return GAMEOBJ:GetObjectByID(targetID)
end
