-- require('State')
--require('o_StateCreate')
require('o_mis')
--require('o_Main')
--[[
Base functionality for wocka ghosts (pathfinding etc.)
v 1.0


]]--

local strCurrentDirection = "N" --default north
local timeCooldown = 0
local bTipGhostStun = false

function onStartup(self) 
	Set = {} -- what does this do?
	self:AddSkill{75}
	self:FollowWaypoints()
	self:SetProximityRadius { radius = 5 , name = "GhostStun" }
	--Set['WanderSpeed']       = 50          -- Move speed 
end


--[[ 
strDir1 and 2 must be capital letters, N, S, E, W, representing the four cardinal directions. 
returns true if strDir1 is directly opposed to strDir2, false otherwise
]]--
function isOppositeDirection(strDir1, strDir2)
	if strDir1 == "N" and strDir2 == "S" then return true end
	if strDir1 == "S" and strDir2 == "N" then return true end
	if strDir1 == "E" and strDir2 == "W" then return true end
	if strDir1 == "W" and strDir2 == "E" then return true end
	
	if strDir2 == "N" and strDir1 == "S" then return true end
	if strDir2 == "S" and strDir1 == "N" then return true end
	if strDir2 == "E" and strDir1 == "W" then return true end
	if strDir2 == "W" and strDir1 == "E" then return true end
	
	return false
end


--[[ 
returns a string N, S, E, W based on my current linear velocity
assumes orientation of X+ being E, X- being W, AND
>>>>>> ***** Z- being N, Z+ being S ******** <<<<<
]]--
function getCurrentCardinalDirection(pSelf)
	local vFacing = pSelf:GetPlayerForward().fwd
	if math.abs(vFacing.x) > math.abs(vFacing.z) then
		-- x is bigger than z
		if vFacing.x > 0 then
			return "E"
		else
			return "W"
		end
	else
		-- z is bigger than x
		if vFacing.z > 0 then
			return "S"
		else
			return "N"
		end
	end
	
	-- should never return this
	return nil
end


--[[ 
called on every waypoint, waypoints are 0 based in Lua script
	Assumptions: 
 	1) EOP actions should never happen on the same waypoint as JXN_X actions.
  	2) EOP action values will always be in the form ""
	
	Actions:
	Name	Values
	EOP	<next_path_name>,<waypoint_index>,<direction_of_jump>
		End Of Path
		
	JXN	<next_path_name>,<waypoint_index>,<direction_of_jump>
		Junction.  Should have one JXN action for every possible jump
]]--
function onArrived(self, msg)
	strCurrentDirection = getCurrentCardinalDirection(self)
	--print("onArrived:", msg.wayPoint, "current direction:", strCurrentDirection) --DEBUG
	
	if msg.actions then
		local s
		local t
		local iNumActions = #msg.actions
		--print(#msg.actions)  -- how many actions
		
		for index, value in ipairs(msg.actions) do
			--print(index, value.name, value.value) --DEBUG
			
			 ----------------------- teleport
			if name == "teleport" then
				local s = value
				local t = split(s, ',')

				local xSplit= {x=t[1] , y=t[2] , z=t[3] }
				self:Teleport{ pos = xSplit}
			    
				--self:SetVar("WPEvent_NUM", self:GetVar("WPEvent_NUM") + 1 ) 
				--setState("WayPointEvent", self) 
		
			--If we reach end of path, transition to next appropriate path and node
			elseif value.name == "EOP" then
				s = value.value
				t = split(s, ',')
				-- t[1] == new path
				-- t[2] == new waypoint
				-- t[3] == new direction
				
				if t[3] ~= nil then	
					if isOppositeDirection(strCurrentDirection, t[3]) then 
						break -- don't change direction if you're just bouncing without a wall in front of you
					else
						strCurrentDirection = string.upper(t[3])
					end
				end
					
				if t[2] == nil then
					t[2] = 0
				end
				
				--print ("At EOP, changing path to:", t[1], t[2], strCurrentDirection) --DEBUG
				
				self:SetVar("attached_path", t[1])		
				self:SetVar("attached_path_start", t[2])
				self:FollowWaypoints()
				break
				
			--If we reach a junction, determine if we should go in a new direction (never in opposite of current direction)
			elseif string.find(value.name,"JXN") ~= nil then				
				-- randomly pick a new direction, % based on number of JXNs (2.0: never reverse direction, will need to store current direction)
				local iChance
				local iRoll
				if iNumActions == 1 then
					iChance = 50
				elseif iNumActions == 2 then
					iChance = 33
				elseif iNumActions == 3 then
					iChance = 25
				else
					iChance = 25
				end
				
				iRoll = math.random(100)
				--print("At JXN, %chance to change:",iChance," rolled a",iRoll) --DEBUG
				if  iRoll < iChance then
					s = value.value
					t = split(s, ',')
					-- t[1] == new path
					-- t[2] == new waypoint
					-- t[3] == new direction
					
					if t[3] ~= nil then	
						if isOppositeDirection(strCurrentDirection, t[3]) then 
							--print("not changing due to opposite direction") --DEBUG
							break -- don't change direction if you're just bouncing without a wall in front of you
						else
							strCurrentDirection = string.upper(t[3])
						end
					end
										
					if t[2] == nil then
						t[2] = 0
					end
					
					--print ("Changing path to:", t[1], t[2], strCurrentDirection) --DEBUG
					
					self:SetVar("attached_path", t[1])
					self:SetVar("attached_path_start", t[2])
					self:FollowWaypoints()
				end
			end
			
			
		end
	end
	
	-- if there are no messages, carry on
	self:ContinueWaypoints();
end


--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
onTimerDone = function(self, msg)
	if (msg.name == "StunCooldown") then
		timeCooldown = 0
	end
end


function onProximityUpdate(self, msg)
	local foundFaction =  msg.objId:GetFaction().faction
	local myFaction = self:GetFaction().faction
	
	if myFaction == foundFaction then return end
	
	print("something near, me:", myFaction, "it:", foundFaction) -- DEBUG
	
	local bSuccess = false
	
	-- need to make sure to addskill onstartup
	--[[CastSkill
	    number skillID
	    OBJECT optionalTargetID
	    bool      succeeded      -- this is set when you call the function.  Check it to see if it succeeded
	    number timeToRecast -- this is set when you call it.  It tells you how long to wait until you can cast it again 
	]]--
	--if timeCooldown == 0 then
		--local timeCooldown = 2
		--msg.objId:SetStunned{true}
		
		-- display tooltip to all players in race
	--[[for pnum = 1, #PLAYERS do
	
	    local player = GAMEOBJ:GetObjectByID(PLAYERS[pnum])
	    local playerData = self:GetVar(PLAYERS[pnum])
	    
	    if ((winner:GetID() == PLAYERS[pnum])) then

	        -- show tooltip to winner
	        player:DisplayTooltip{ bShow = true, strText = "You Win!\n Waiting for other players." }
	        player:PlayFXEffect{effectType = "fireworks"}

	    elseif (player and playerData) then
	    
	        -- show tooltip to others
	        player:DisplayTooltip{ bShow = true, strText = "Winner: " .. winnerName .. "\nRace ends in " .. CONSTANTS["RACE_FINISH_TIME"] .. " seconds.", iTime = 5000 }

	    end

	end]]--

		--optionalTargetID = self, 
		self:CastSkill{skillID = 75 ,succeeded = bSuccess, timeToRecast = timeCooldown} -- sonic boom stun skill
--~ 		if not bTipGhostStun then
--~ 			print ("tip")
--~ 			--local player = GAMEOBJ:GetObjectByID(msg.objId)
--~ 			--print("player id:",player:GetID())
--~ 			msg.objId:DisplayToolTip{bShow=true, strText = "Watch out!  Ghosts stun you!"}
--~ 			bTipGhostStun = true
--~ 		end
		--self:CastSkill{skillID = 54 ,optionalTargetID = msg.objId, succeeded = bSuccess, timeToRecast = timeCooldown} -- sonic boom stun skill
		--print("timeCooldown", timeCooldown, "bsuccess", bSuccess) -- DEBUG
		--GAMEOBJ:GetTimer():AddTimerWithCancel( timeCooldown, "StunCooldown", self )
	--end
	
      --[[


     if msg.objType == "Enemies" or msg.objType == "NPC" or msg.objType == "Rebuildables" then

----------------------------------------------------------------------------------
                
         
---------------------------------------------------------------------------------
                -- Follow Code -- 
                if foundFaction == 1 and self:GetVar("Set.FollowActive")  and  (self:GetVar("Imfollowing") == nil or self:GetVar("Imfollowing") == false)  and self:GetVar("Imfollowing") ~= "done" then
                    if  msg.status == "ENTER" and msg.name == "conductRadius" then
                        storeHomePoint(self)
                        storeTarget(self, msg.objId)
                        setState("Follow", self)
                    end
                end 

            --print(msg.objId:GetName().name) 
            -- Fear Flee onProx
            if ( self:GetVar("Set.FearPlayer") or self:GetVar("Set.FearNPC") ) and not self:GetVar("inpursuit") and self:GetVar("tetherON") ~= true and not self:GetVar("Fleeing") then
                -- Chance to Flee -- 
                local  ran =  math.random(1,100)
                if ran <= self:GetVar('Set.FearChance') then
                    -- Get Player Faction here -- 
                    if self:GetVar('Set.FearPlayer') and msg.objId:GetFaction().faction == 1 then
                        self:SetVar("FearFound", true)
                    end
                    -- Get NPC Faction List here -- 
                    if self:GetVar('Set.FearNPC') then
                        for u = 1,4 do  
                            if msg.objId:GetFaction().faction == self:GetVar("Set.FearNPC_"..u) then
                                self:SetVar("FearFound", true)
                            end
                        end 
                    end 
                   -- Store target if found 
                    if  self:GetVar("FearFound") then 
                            storeTarget(self, msg.objId)
                            
                    end

                    if msg.status == "ENTER" and msg.name == "conductRadius" and not self:GetVar("FearFlee_CoolDown") and self:GetVar("FearFound") and self:GetVar("Set.FearFOV") == nil then
                            GAMEOBJ:GetTimer():CancelAllTimers( self )
                            self:SetTetherPoint { tetherPt = self:GetPosition().pos,radius = self:GetVar("Set.tetherRadius") }
                            setState("FearFlee",self)
                    end
                    if msg.status == "ENTER" and msg.name == "conductFOVRadius" and not self:GetVar("FearFlee_CoolDown") and self:GetVar("FearFound") and  self:GetVar("Set.FearFOV") > 0 then
                            local infront = self:IsObjectInFOV { target = msg.objId, radius = self:GetVar("Set.FearFOV") , minRange = 0, maxRange = 100 }.result
                            if infront then
                                GAMEOBJ:GetTimer():CancelAllTimers( self )
                                self:SetTetherPoint { tetherPt = self:GetPosition().pos,radius = self:GetVar("Set.tetherRadius") }
                                setState("FearFlee",self)
                            end
                    end
                end
            end 



           if msg.status == "ENTER" and (self:GetVar("Set.Conduct_1_Active") or self:GetVar("Set.FollowActive")) and not self:GetVar("inpursuit") then

                local foundFaction = msg.objId:GetFaction().faction


                if foundFaction == self:GetVar("Set.Con_1_AFaction") and self:GetVar("Set.Con_1_Type") == "sneakto" and self:GetVar("Set.Conduct_1_Active") then
                        self:SetVar("FoundFOVtarget", true) 
                
                elseif foundFaction == self:GetVar("Set.Con_2_AFaction") and self:GetVar("Set.Con_2_Type") == "sneakto" and self:GetVar("Set.Conduct_2_Active") then
                        self:SetVar("FoundFOVtarget", true) 
                else
                        self:SetVar("FoundFOVtarget", false) 
                end 

                
                if msg.name == "conductRadius" and msg.objId:GetVar("inpursuit") ~= true and not self:GetVar("inpursuit") and not self:GetVar("FoundFOVtarget") then

                    if (foundFaction == self:GetVar("Set.Con_1_AFaction") and self:GetVar("Set.Con_1_Type") ~= "sneakto") or (foundFaction == self:GetVar("Set.Con_2_AFaction") and self:GetVar("Set.Con_2_Type") ~= "sneakto")then
                
                        local ran = math.random(1,100)
                     
                        if self:GetVar("Set.Conduct_MainWeight") >= ran then
                            self:SetVar("ConductCoolingDown",true)
                            self:SetVar("inpursuit", true) 
                            GAMEOBJ:GetTimer():CancelAllTimers( self )
                            self:SetTetherPoint { tetherPt = self:GetPosition().pos,radius = self:GetVar("Set.tetherRadius") }
                            storeHomePoint(self)
                            storeTarget(self, msg.objId)
                            conductTrigger(self,msg.objId:GetFaction().faction) 
                         end 
                     
                    end
                end
                    
            end 
             if msg.name == "conductFOVRadius" and not msg.objId:GetVar("inpursuit") and not self:GetVar("inpursuit") and not self:GetVar("ConductCoolingDown") then
            
                if msg.objId:GetFaction().faction ==  self:GetVar("Set.Con_1_AFaction") or msg.objId:GetFaction().faction ==  self:GetVar("Set.Con_2_AFaction") and msg.status == "ENTER" then
                
                    local ran = math.random(1,100)
                   
                    
                        if self:GetVar("Set.Conduct_MainWeight") >= ran then

                            local infront = self:IsObjectInFOV { target = msg.objId, radius = self:GetVar("Set.conductFOV") , minRange = 0, maxRange = 100 }.result
                            if infront then
                                self:SetVar("ConductCoolingDown",true)
                                self:SetVar("inpursuit", true) 
                                GAMEOBJ:GetTimer():CancelAllTimers( self )
                                self:SetTetherPoint { tetherPt = self:GetPosition().pos,radius = self:GetVar("Set.tetherRadius") }
                                storeHomePoint(self)
                                storeTarget(self, msg.objId)
                                conductTrigger(self,msg.objId:GetFaction().faction) 
                            end
                            
                        end
                
                  end
                
              end            
           
            
            

------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------



             --------- Helper On Enter
             if msg ~= nil and checkFaction == 0 then
                 if msg.status == "ENTER" and msg.objId:GetFaction().faction == self:GetVar("HelpFaction") and not msg.objId:IsDead().bDead and msg.name == "HelpRadius" and msg.objId:GetVar("inpursuit") == true and self:GetVar("inpursuit") == false then
                        target = getMyTarget(msg.objId)
                      --  print("My Target is ==="..target)
                         if target ~= nil then
                            storeTarget(self, target)
                         else
                            print("Error i dont have a target") 
                         end
                        
                        setState("aggro", self)
                       
                 end
             end
             --- Aggro On Enter

                -- Get Hate List
               if self:GetVar("Set.AggroNPC") then
                  for i = 1, 4 do 
                    if msg.objId:GetFaction().faction == self:GetVar("Set.NPCHated_"..i) then
                        self:SetVar("FoundHated", true) 
                    end                   
                  end
               end


               if msg.status == "ENTER" and (self:IsEnemy{ targetID = msg.objId }.enemy or self:GetVar("FoundHated"))   and not msg.objId:IsDead().bDead and msg.name == "aggroRadius" and self:GetVar("FleeStatus")  ~= 1 then
                    GAMEOBJ:GetTimer():CancelAllTimers( self )
                    if self:GetVar("Set.Aggression") == "Aggressive" or self:GetVar("Set.Aggression") == "PassiveAggres" then
                            storeTarget(self, msg.objId)
                            
                            if self:GetVar("aggrotarget") == 0 and self:GetVar("AggroDelayDone") then

                                  if self:GetVar("Set.AggroEmote") and self:GetVar("AggroDelayDone") then
                                  
                                  
                                     self:FaceTarget{ target = getMyTarget(self), degreesOff = 5, keepFacingTarget = true }
                                     Emote.emote(self,target , self:GetVar("Set.AggroE_Type")) 


                                    GAMEOBJ:GetTimer():CancelAllTimers( self )
                                    self:SetVar("inpursuit", true) 
                                    self:SetVar("aggrotarget",1) 
                                    storeHomePoint(self)
                                    self:SetTetherPoint { tetherPt = myPos,
                                                          radius = self:GetVar("tetherRadius") 
                                                        }
                                     GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("Set.AggroE_Delay") , "AggroEmote", self )
                                     self:SetVar("AggroDelayDone", false) 
                                     setState("Idle",self)
                                     
                                else

                                    GAMEOBJ:GetTimer():CancelTimer( "Conduct", self )
                                    self:SetVar("inpursuit", true) 
                                    storeHomePoint(self)
                                    local myPos = self:GetPosition().pos
                                    self:SetTetherPoint { tetherPt = myPos,
                                                          radius = self:GetVar("Se.tetherRadius") 
                                                        }
                                    self:SetVar("aggrotarget",1) 
                                end
                            end
                            if aggroTarget ~= 2 and self:GetVar("AggroDelayDone") then
                                setState("aggro", self)
                            end
                    end
                end


       end
       ]]--
end






















