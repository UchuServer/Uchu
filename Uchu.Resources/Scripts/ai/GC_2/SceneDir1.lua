require('o_mis')
require('State')

function onStartup(self)

    self:SetName { name = "Scene Dir"  }
    self:SetProximityRadius { radius = 500 , name = "ALL" }
  
    
    self:SetVar("Mission_A_State", "idle") 
    self:SetVar("Mission_B_State", "idle") 
    
    
    self:UseStateMachine{} 
    
    Idle = State.create()
    Idle.onEnter = function(self)
     local foundObj = self:GetProximityObjects { name = "ALL"}.objects
            for i = 1, table.maxn (foundObj) do  
                if foundObj[i]:GetName().name == "Trigger_1" or foundObj[i]:GetName().name == "Contest Brick Walker" or foundObj[i]:GetName().name == "Contest Red Brick Walker" or foundObj[i]:GetName().name == "Contest Scene Dir Main" then
                 StoreOutHouse(  foundObj[i],  self)  	
                end
                
            end 
    end
    Idle.onArrived = function(self)

    end    
    Stage1 = State.create()
    Stage1.onEnter = function(self)

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

    end    
     Stage3.onArrived = function(self)

    end 
   
    addState(Stage1, "Stage1", "Stage1", self)
    addState(Stage2, "Stage2", "Stage2", self)
    addState(Stage3, "Stage3", "Stage3", self)
    addState(Idle, "Idle", "Idle", self)
    beginStateMachine("Idle", self) 


    
    
end




