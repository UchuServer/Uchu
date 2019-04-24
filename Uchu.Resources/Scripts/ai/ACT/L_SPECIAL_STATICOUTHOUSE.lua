require('State')
require('o_onEvent')
require('o_mis')
--///////////////////////////////////////////////////////////////////////////////////////
--//            OUTHOUSE -- Script
--///////////////////////////////////////////////////////////////////////////////////////

function onStartup(self) 

    self:SetVar("OutHouseState", 1) --- 1 hole -- 2 broken -- 3 Build Complted 
    self:UseStateMachine{} 
    
    self:SetName { name = "OutHouse"  }
   -- Rotate an object by a given number of degrees


    HouseDead = State.create()

    HouseDead.onEnter = function(self)
     
      self:KillObj{ targetID = self }    
             

    end    

    -- //////////////////////////////////////////////////////////////////////////////////
    -- Idle State
    -- //////////////////////////////////////////////////////////////////////////////////

    ParentIdle = State.create()
    ParentIdle.onEnter = function(self)
         if self:GetVar("OutHouseState") == 1 then
            GAMEOBJ:GetTimer():AddTimerWithCancel( 3, "BreakOuthouse",self )
         end

    end 
    ParentIdle.onArrived = function(self)
        
    end  
  
    -- //////////////////////////////////////////////////////////////////////////////////
    -- Load Childern State
    -- //////////////////////////////////////////////////////////////////////////////////
        LoadChildren = State.create()
        LoadChildren.onEnter = function(self) 
          
           
        end
        LoadChildren.onArrived = function(self)
        
        end  
    --------------------------------------------------------------------------------------
    
        
    
    addState(HouseDead, "HouseDead", "HouseDead", self)
    addState(ParentIdle, "ParentIdle", "ParentIdle", self)
    addState(LoadChildren,"LoadChildren","LoadChildren",self) 
    beginStateMachine("ParentIdle", self) 
    ParentIdle.onEnter(self)
end
     
