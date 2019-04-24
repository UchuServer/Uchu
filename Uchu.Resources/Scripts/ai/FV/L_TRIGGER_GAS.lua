require('o_mis')
require('State')


function onCollisionPhantom(self, msg)
		 if not  self:GetVar("Trg") then
	        self:SetVar("Trg.1", 0 )
	     end
		 local target = msg.objectID
	     if ( target:GetFaction().faction == 1 ) then 
	     
	     
	     
	                local t = self:GetVar("Trg")
	                -- Save Target ID's
	               for i = 1, table.maxn( t ) do 
	                    if self:GetVar("Trg."..i) ==  nil or self:GetVar("Trg."..i) == 0  then
	                        storePlayer(self, target , i) 
	                        self:SetVar("running", true) 
	                        setState("aggro", self)
	                        break
	                    end
	                end     
        end 
        
       
end
function onOffCollisionPhantom (self,msg)

	 local target = msg.senderID
	     if ( target:GetFaction().faction == 1 ) then 

           for i = 1, table.maxn( self:GetVar("Trg")) do 
             if getPlayer(self, i):GetName().name ==  target:GetName().name then
               self:SetVar( "Trg."..i, nil )
             
               
                break
             end 
	    end 

	end  
	

end



 function CanAttackTarget(self , target)
        local myTarget = target
        local range = 50
        local myPos = Vector.new(self:GetPosition().pos)
        local hisPos = Vector.new(target:GetPosition().pos)
        local dist = hisPos - myPos
        
       if not target:Exists() or target:IsDead().bDead then
          return false
       end
       
     
        if dist:sqrLength() <= range * range then 
                return true
        end
        return false
        
    end -- end CanAttackTarget



function onStartup(self)

    self:SetVar("running", false) 
  

     -- Create Player Table
        Trg = {}
      
        Trg[1] = 0
        self:SetVar("Trg",Trg)
        
     --###########################################################
     --########## STATES
     --###########################################################
      -- Idle
      Idle = State.create()
      Idle.onEnter = function(self)
             
      end 
      Idle.onArrived = function(self)
             
      end 
      -- Aggro   
      aggro = State.create()
      aggro.onEnter = function(self)
      
     if self:GetVar("Trg") then
           for i = 1, table.maxn( self:GetVar("Trg")) do 
                	local Player = getPlayer(self, i)
                    
                    local item = Player:GetEquippedItemInfo{ slot = "hair" }.lotID 
                    if  item ~= 3068 then
                        self:CastSkill{ optionalTargetID = Player ,skillID = 103 } 
                    end 
                    if (Player:IsDead().bDead) then
                       for i = 1, 10 do  
                         if getPlayer(self, i) ==  Player then
                            table.remove(self:GetVar("Trg"), i)

                         
                            break
                         end 
                       end
                    end 
               
            end 
        
        end
       
        local theTime = GAMEOBJ:GetTimer():GetTime("timerAggro",self )
       -- print(theTime)
        if self:GetVar("running") == true and self:GetVar("Trg") and theTime == 0  then
           GAMEOBJ:GetTimer():AddTimerWithCancel(  3 , "timerAggro", self )
       else
       
      	self:SetVar("running", false)
      	end
     
     
      end 
      
      aggro.onArrived = function(self)
             
      end 
   
      addState(Idle, "Idle", "Idle", self)
      addState(aggro, "aggro", "aggro", self)
      beginStateMachine("Idle", self)
      
      
end
function getPlayer(self, num)
    targetID = self:GetVar("Trg."..num )
    return GAMEOBJ:GetObjectByID(targetID)
end

function storePlayer(self, target, num)
    idString = target:GetID()
    finalID = "|" .. idString
    self:SetVar("Trg."..num , finalID)
end

     
onTimerDone = function(self, msg)

    if msg.name == "timerAggro" then 
      setState("aggro",self)
    end
    
end 


