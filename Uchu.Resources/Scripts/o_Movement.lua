function MeanderState(self)

        storeMeanderPoint(self)
      --  GetPlayAction(self)
        Meander = State.create()        
        Meander.onEnter = function(self)
         self:SetVar("CurrentState", "Meander")
                radius = self:GetVar("Set.wanderRadius") 
                myPos = getMeanderPoint(self)
                PoS = getRandomPos(self,myPos,radius)
    
                self:GoTo { speed = self:GetVar("Set.WanderSpeed"),
                            target = { 
                                 x = PoS.x ,
                                 z = PoS.z ,
                                 y = PoS.y,
                           },
                   }
                
                end
             Meander.onArrived = function(self)
                 local ran = getRandomDelay(self)
                 GAMEOBJ:GetTimer():CancelTimer( "MeanderPause", self )

                 if ran < 1 then
                     local ran = 1 
                 end 
   
             GAMEOBJ:GetTimer():AddTimerWithCancel( ran, "MeanderPause", self )
           
              if self:GetVar("Set.WanderEmote") then
                    local ranc = math.random(1,100)
                    if ranc <= self:GetVar("Set.WanderChance") then     
                        if self:GetVar('Set.WEmote_2') then
                           local ran = math.random(1,100)
                           local effect_2 = 100 - self:GetVar('Set.WEmote_1')

                                   if self:GetVar('Set.WEmote_1') < 50 then
                                      if ran > 0  and ran <= self:GetVar('Set.WEmote_1') then
                                         local oEffect = self:GetVar("Set.WEmoteType_1")
                                         local oType = self:GetVar("Set.WEmoteEffe_1")
                                         PlayEmote(self,1)   
                                      else 
                                          local oEffect = self:GetVar("Set.WEmoteType_2")
                                          local oType = self:GetVar("Set.WEmoteEffe_2") 
                                          PlayEmote(self,2)                             
                                      end
                                  elseif effect_2 <= 50 then
                                      if ran > effect_2 and ran < 100 then
                                         local oEffect = self:GetVar("Set.WEmoteType_1")
                                         local oType = self:GetVar("Set.WEmoteEffe_1") 
                                         PlayEmote(self,1)
                                      else 
                                          local oEffect = self:GetVar("Set.WEmoteType_2")
                                          local oType = self:GetVar("Set.WEmoteEffe_2") 
                                          PlayEmote(self,2)                             
                                      end
                                   end
                       end
                       if not self:GetVar('Set.WEmote_2') then
                           local oEffect = self:GwetVar("Set.WEmoteType_1")
                           local oType = self:GetVar("Set.WEmoteEffe_1")  
                           Emote.emote(self,self,oEffect)
                        end 
                       
                    end
           
                end
           end 
        AggroState(self)
 end 
function FollowState(self)

     Follow = State.create()   
     Follow.onEnter = function(self)
      self:SetVar("CurrentState", "Follow")
     local foundObj = self:GetProximityObjects { name = "Followers"}.objects
              local Found = {}
              local PointCount = 0 
              local LineCount = 0
              local max = 0
              for i = 1, table.maxn (foundObj) do  
                   if foundObj[i]:BelongsToFaction{factionID = 7}.bIsInFaction then
                                            
                                 if  foundObj[i]:GetVar("PointInLine") then 
                                  self:SetVar("PointInLine", true) 
                                  foundObj[i]:SetVar("PointInLine", false) 
                                  local idString = foundObj[i]:GetID()
                                  local finalID = "|" .. idString     
                                  self:SetVar("EndOfLineTarget", finalID) 
                       
                                 end                         
                         
                    end
              end
            
               self:SetName { name = "scared"  }
                 if self:GetVar("EndOfLineTarget") ~= nil then
                     self:SetVar("Imfollowing", true) 
                     if GAMEOBJ:GetObjectByID(self:GetVar("EndOfLineTarget")):GetVar("Imfollowing") ~= "done" then 
                        emote(self, self , "flip" )
                        self:FollowTarget { targetID = GAMEOBJ:GetObjectByID( self:GetVar("EndOfLineTarget")) , radius = 3 ,speed = 3, keepFollowing = true }
                     else
                            emote(self, self , "flip" )
                            self:SetVar("Imfollowing", true) 
                            self:FollowTarget { targetID = getMyTarget(self) , radius = 3 ,speed = 3, keepFollowing = true }
                     end
                 else
                     emote(self, self , "flip" )
                     self:SetVar("PointInLine", true)
                     self:SetVar("Imfollowing", true) 
                     self:FollowTarget { targetID = getMyTarget(self) , radius = 3 ,speed = 3, keepFollowing = true }
                    
                 end 
              
     
       -- Find all Followers
       -- Follow last Target
       -- table.remove (table)
     end
          
     Follow.onArrived = function(self)

     end 

     AggroState(self)
end























function PlayEmote(self,i)  

    local oEffect = self:GetVar("Set.WEmoteType_"..i)
    local oType = self:GetVar("Set.WEmoteEffe_"..i)  
    Emote.emote(self,self,oEffect)

end
