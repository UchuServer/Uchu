function onProximityUpdate(self, msg)
 
           
 if msg.objType == "Enemies" or msg.objType == "NPC" then
            
            -- Fear Flee onProx
            if ( self:GetVar("Set.FearPlayer") == true or self:GetVar("Set.FearNPC") == true ) and self:GetVar("inpursuit") == false then
                -- Chance to Flee -- 
                local  ran =  math.random(1,100)
                if ran <= self:GetVar('FearChance') then
                    -- Get Faction List here -- 
                    for u = 1,5 do  
                        if msg.objId:BelongsToFaction{factionID = self:GetVar("FearNPC_"..u)}.bIsInFaction then
                            self:SetVar("FearFound", true) 
                        end
                    end                    


                    if msg.status == "ENTER" and msg.name == "conductRadius" and self:GetVar("FearFlee_CoolDown") == false then
                            local myPos = self:GetPosition().pos
                            self:SetTetherPoint { tetherPt = myPos,
                                                  radius = self:GetVar("Set.tetherRadius") 
                                                }
                            
                            storeTarget(self, msg.objId)
                            setState("FearFlee",self)
                    end
                end
            end 

            if self:GetVar("Set.Conduct_1_Active") and msg.status == "ENTER" and msg.name == "conductRadius" then



            end




------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------
--]] 

             if self:IsEnemy{ targetID = msg.objId }.enemy == true and not msg.objId:IsDead().bDead and self:GetVar("FleeActive") == true and self:GetVar("FleeStatus")  == 0 and msg.objId:GetVar("inpursuit") == true then
                 health = self:GetHealth{}.health 

                 if health <= self:GetVar("FleeHealth")  then
                   self:SetVar("FleeStatus",1)  
                    setState("Flee",self)
                 end

             end
             --------- Helper On Enter
             if msg ~= nil and checkFaction == 0 then
                 if msg.status == "ENTER" and msg.objId:BelongsToFaction{factionID = self:GetVar("HelpFaction")}.bIsInFaction and not msg.objId:IsDead().bDead and msg.name == "HelpRadius" and msg.objId:GetVar("inpursuit") == true and self:GetVar("inpursuit") == false then
                        target = getMyTarget(msg.objId)
                      --  print("My Target is ==="..target)
                         if target ~= nil then
                            storeTarget(self, target)
                         else
                            print("Error i dont have a target") 
                         end
                        
                        if self:GetVar("Set.SuspendLuaAI") == true then
							setState("combat",self)
							return
						end
                        setState("aggro", self)
                       
                 end
             end
             --- Aggro On Enter
               if msg.status == "ENTER" and self:IsEnemy{ targetID = msg.objId }.enemy  and not msg.objId:IsDead().bDead and msg.name == "aggroRadius" and self:GetVar("FleeStatus")  ~= 1 then
                    GAMEOBJ:GetTimer():CancelTimer( "PatDelay", self )
                    GAMEOBJ:GetTimer():CancelTimer( "MeanderPause", self )
                    if self:GetVar("Set.Aggression") == "Aggressive" then
                            storeTarget(self, msg.objId)
                             
                            if self:GetVar("aggrotarget") == 0 then
                                GAMEOBJ:GetTimer():CancelTimer( "Conduct", self )
                                self:SetVar("inpursuit", true) 
                                storeHomePoint(self)
                               local myPos = self:GetPosition().pos
                                self:SetTetherPoint { tetherPt = myPos,
                                                      radius = self:GetVar("Se.tetherRadius") 
                                                    }
                                self:SetVar("aggrotarget",1) 
                            end
                            if aggroTarget ~= 2 then
								if self:GetVar("Set.SuspendLuaAI") == true then
									setState("combat",self)
									return
								end
                                setState("aggro", self)
                            end
                    end
                end
                --------------------- Conduct 
                if  self:GetVar("inpursuit") ~= true and self:GetVar("ConductActive") == true and  self:GetVar("ConductStarted") == false and self:GetVar("ConductCooldown") == false then
                    --- Conduct On Enter
                    if msg.status == "ENTER" and msg.objId:BelongsToFaction{factionID = 1}.bIsInFaction and msg.name == "ConductRadius" and not msg.objId:IsDead().bDead and not self:IsDead().bDead then
                        local ranDelay =  math.random(self:GetVar("ConductChanceMin"),self:GetVar("ConductChanceMax"))
                        local ran = math.random(1,100)
                        if ran <= self:GetVar("Meander_Chance") then               
                            storeTarget(self, msg.objId)
                            storeConductHomePoint(self)
                            self:SetVar("ConductStarted",true)
                            GAMEOBJ:GetTimer():AddTimerWithCancel( ranDelay, "Conduct", self )
                        end  
                    end
                 end
                 --- Conduct On Exit
                 if self:GetVar("ConductActive") == true and self:Exists() and msg ~= nil then
                    
                        if msg.status == "LEAVE" and msg.objId:BelongsToFaction{factionID = 1}.bIsInFaction and msg.name == "ConductRadius" and not msg.objId:IsDead().bDead and self:Exists() and not self:IsDead().bDead then
                            self:SetVar("ConductStarted",false)
                           if self:Exists() then
                                GAMEOBJ:GetTimer():CancelTimer( "Conduct", self )
                                GAMEOBJ:GetTimer():CancelTimer( "ConductPause", self )
                           end
                     end
              end
       end
end



--******************************************************************************
-- on Hit

function onOnHit(self,msg)

	if self:GetVar("Set.SuspendLuaAI") == "true" then
		setState("combat", self)
		return
    end
    
     if not msg.attacker:IsDead().bDead and self:IsEnemy{ targetID = msg.attacker }.enemy == true and self:GetVar("inpursuit") == true then
        self:SetVar("FleeStatus",1)  
        setState("Flee",self)
    
    end

     if self:IsEnemy{ targetID = msg.attacker }.enemy == true and not msg.attacker:IsDead().bDead and not self:IsDead{}.bDead  then
        GAMEOBJ:GetTimer():CancelAllTimers( self )
        storeTarget(self, msg.attacker)
        aggroTarget = self:GetVar("aggrotarget") 
        local myPos = self:GetPosition().pos

-------------------------------------------------------------- 

        if self:GetVar("isHelper") == true and not msg.attacker:IsDead().bDead and not self:IsDead().bDead then 
              npcs = {}
              npcs = self:GetProximityObjects { name = "HelpRadius", type = "Enemies" }.objects
              for u = 1,table.maxn(npcs) do
                if not msg.attacker:IsDead().bDead and not self:IsDead().bDead and table.maxn(npcs) >= 1 then 
                        if not npcs[u]:IsDead{}.bDead and npcs[u]:BelongsToFaction{factionID = self:GetVar("HelpFaction")}.bIsInFaction and npcs[u]:GetVar("inpursuit") == false then
                            storeTarget(npcs[u], msg.attacker)
                            storeHomePoint(npcs[u])                   
                            if npcs[u]:Exists() and self:Exists() then
                                GAMEOBJ:GetTimer():CancelAllTimers( npcs[u] )
                                npcs[u]:SetVar("inpursuit", true)
                                npcs[u]:SetState { stateName = "aggro" }
                            end
                       end
                  end
             end
        end

-----------------------------------------------------------------
        if aggroTarget == 0 and not self:IsDead().bDead then
            storeHomePoint(self)
            local myPos = getHomePoint(self)
            GAMEOBJ:GetTimer():CancelTimer( "Conduct", self )
            GAMEOBJ:GetTimer():CancelTimer( "MeanderPause", self )
            self:SetVar("inpursuit", true) 
          
            self:SetVar("aggrotarget",1) 
            self:SetTetherPoint { tetherPt = myPos,
                                  radius = self:GetVar("tetherRadius") 
                                }
        end
        if aggroTarget ~= 2 and not self:IsDead().bDead then
			if self:GetVar("Set.SuspendLuaAI") == true then
				setState("combat",self)
				return
			end
            setState("aggro", self)
        end

    end

end