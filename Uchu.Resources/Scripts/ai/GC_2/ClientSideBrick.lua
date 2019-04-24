

function IsLocalCharacter(target)

    return GAMEOBJ:GetLocalCharID() == target:GetID()

end
function onStartup(self)
self:SetProximityRadius { radius = 30, name = "clientRadius" } 

end

function onProximityUpdate(self, msg) 
--//////////////////////////////////////////////////////////////////////////////////
--//////////////////////////////////////////////////////////////////////////////////
--//////////////////////////////////////////////////////////////////////////////////
        if  msg.name == "clientRadius" and msg.status == "ENTER" and IsLocalCharacter(msg.objId) and msg.objId:GetFaction().faction == 1 then
                if self:GetVar("FollowState") == nil then
                
                   local ran = math.random(1,30)
                  if ran == 20 then
                      self:DisplayChatBubble{wsText =  "Help!" }
                      self:PlayFXEffect {priority = 1.2, effectType = "scared"}
                  end
                end 

        end
--//////////////////////////////////////////////////////////////////////////////////
--//////////////////////////////////////////////////////////////////////////////////
--//////////////////////////////////////////////////////////////////////////////////  
end


function emote(self,target, skillType)
       
        
        self:PlayFXEffect {priority = 1.2, effectType = skillType}
end
 





