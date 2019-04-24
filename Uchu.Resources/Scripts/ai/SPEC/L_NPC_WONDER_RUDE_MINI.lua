require('State')
require('o_onEvent')
require('o_mis')
--///////////////////////////////////////////////////////////////////////////////////////
--//            OUTHOUSE -- Script
--///////////////////////////////////////////////////////////////////////////////////////

function onStartup(self) 
    self:SetName { name = "RudePeer"  }
    self:SetVar("Enter_WP",1)  
    self:SetImmunity{ immunity = true }
    self:UseStateMachine{} 
    -- //////////////////////////////////////////////////////////////////////////////////
    -- Idle State
    -- //////////////////////////////////////////////////////////////////////////////////

    RudeIdle = State.create()
    RudeIdle.onEnter = function(self)
    
    end 

  
    -- //////////////////////////////////////////////////////////////////////////////////
    -- Enter Out House
    -- //////////////////////////////////////////////////////////////////////////////////
    OutHouse = State.create()
    OutHouse.onEnter = function(self) 
            storeHomePoint(self)
            WayPoint = {}
            ParPos = getParentHomePoint(self)
            WayPoint[1] = { x = ParPos.x - 3.1 ,y = ParPos.y ,z = ParPos.z + 5}
            WayPoint[2] = { x = ParPos.x + 10 ,y = ParPos.y ,z = ParPos.z + 8 }
            WayPoint[3] = { x = ParPos.x + 10, y = ParPos.y ,z = ParPos.z + 10 }

                if self:GetVar("Enter_WP") == 1 then

                    self:GoTo { speed = 1,
                    target = { x = WayPoint[1].x,
                               z = WayPoint[1].z,
                               y = WayPoint[1].y,
                             }
                   }

                end
          
        end
        OutHouse.onArrived = function(self)
        
         if self:GetVar("Enter_WP") == 1 then
             
             self:RotateByDegrees{ speed = 100, degrees = -25 } 
             Emote.emote(self,self, "outhouse-start") 
             OutHouse2 = getOutHouse(self)
             Emote.emote(OutHouse2 ,OutHouse2 , "start") 
             GAMEOBJ:GetTimer():AddTimerWithCancel( 6, "OutHouseEnter",self )
             GAMEOBJ:GetTimer():AddTimerWithCancel( 2, "CallSmoke",self )
             setState("RudeIdle",self) 

         end
        end  
    -- //////////////////////////////////////////////////////////////////////////////////
    -- Exit Out House
    -- //////////////////////////////////////////////////////////////////////////////////
        exitOutHouse = State.create()
        exitOutHouse.onEnter = function(self)
          local target = getMyTarget(self)


          self:FollowTarget { targetID = target,radius = 1, speed = 1 }
          self:GoTo { speed = 1,
                    target = { x = getHomePoint(self).x,
                               z = getHomePoint(self).z,
                               y = getHomePoint(self).y,
                             }
                   } 
        
        end
        exitOutHouse.onArrived = function(self)
             
             self:RotateByDegrees{ speed = 100, degrees = 180 }
             self:SetVar("Enter_WP",1)   
             setState("RudeIdle",self)
        end  



    --------------------------------------------------------------------------------------
    
    addState(OutHouse,"OutHouse","OutHouse",self)    
    addState(exitOutHouse,"exitOutHouse","exitOutHouse",self)    
    addState(RudeIdle, "RudeIdle", "RudeIdle", self)
    beginStateMachine("RudeIdle", self) 
    RudeIdle.onEnter(self)
end
 

function test(self)

setState("enterOutHouse",self)
end    
