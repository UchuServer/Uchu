require('State')
require('o_onEvent')
require('o_mis')
--///////////////////////////////////////////////////////////////////////////////////////
--//            OUTHOUSE -- Script
--///////////////////////////////////////////////////////////////////////////////////////

function onStartup(self) 
    self:SetVar("OutHouseParent",true)
    self:SetVar("Spawn_ID",1875)
    self:SetVar("OutHouse", 12)
    self:SetVar("RudPeer", 12)
    storeHomePoint(self)
    self:SetImmunity{ immunity = true }
    self:UseStateMachine{} 
    -- //////////////////////////////////////////////////////////////////////////////////
    -- Idle State
    -- //////////////////////////////////////////////////////////////////////////////////

    ParentIdle = State.create()
    ParentIdle.onEnter = function(self)
        Emote.emote(self,self, "idle") 
    
    end 
    ParentIdle.onArrived = function(self)

    end  
  
    -- //////////////////////////////////////////////////////////////////////////////////
    -- Load Childern State
    -- //////////////////////////////////////////////////////////////////////////////////
        LoadChildren = State.create()
        LoadChildren.onEnter = function(self) 
          
            RESMGR:LoadObject { objectTemplate = 1875 ,
                                 x= self:GetPosition().pos.x + 4 , 
                                 y=self:GetPosition().pos.y , 
                                 z=self:GetPosition().pos.z + 9 , 
                                 owner = self }
             RESMGR:LoadObject { objectTemplate = 1810 ,
                                 x= self:GetPosition().pos.x - 2 , 
                                 y=self:GetPosition().pos.y , 
                                 z=self:GetPosition().pos.z - 9 , 
                                 owner = self }         
        end
        LoadChildren.onArrived = function(self)
        
        end  
    --------------------------------------------------------------------------------------
    
        
    addState(ParentIdle, "ParentIdle", "ParentIdle", self)
    addState(LoadChildren,"LoadChildren","LoadChildren",self) 
    beginStateMachine("LoadChildren", self) 
    LoadChildren.onEnter(self)

 end    
