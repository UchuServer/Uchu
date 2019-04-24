require('o_mis')
require('client/ai/AG/L_AG_NPC')

function onStartup(self,msg)

    self:SetVar("MGState", 1)
    self:SetProximityRadius { radius = 20 }

end

function onProximityUpdate(self, msg)

	local target = msg.objId
	local faction = target:GetFaction()

    if faction and faction.faction == 1 and self:GetVar("MGState") == 1 then
        if msg.status == "ENTER" then
            self:PlayAnimation{ animationID = "missionState1" }
            self:DisplayChatBubble {wsText = "I need your help!"}
        elseif msg.status == "LEAVE" then
            self:PlayAnimation{ animationID = "idle" }
            self:DisplayChatBubble {wsText = "I still need your help!"}
        end
    
    elseif faction and faction.faction == 1 and self:GetVar("MGState") == 2 then
        if msg.status == "ENTER" then
            self:PlayAnimation{ animationID = "missionState2" }
            self:DisplayChatBubble {wsText = "Why aren't you done yet?!"}
        elseif msg.status == "LEAVE" then
            self:PlayAnimation{ animationID = "idle" }
            self:DisplayChatBubble {wsText = "Get 'er done!"}
        end

    elseif faction and faction.faction == 1 and self:GetVar("MGState") == 3 then
        if msg.status == "ENTER" then
            self:PlayAnimation{ animationID = "missionState3" }
            self:DisplayChatBubble {wsText = "You did it!"}
        elseif msg.status == "LEAVE" then
            self:PlayAnimation{ animationID = "idle" }
            self:DisplayChatBubble {wsText = "You are awesome!"}
        end
    
    elseif faction and faction.faction == 1 and self:GetVar("MGState") == 4 then
        if msg.status == "ENTER" then
            self:PlayAnimation{ animationID = "missionState4" }
            self:DisplayChatBubble {wsText = "Thanks for the help!"}
        elseif msg.status == "LEAVE" then
            self:PlayAnimation{ animationID = "idle" }
            self:DisplayChatBubble {wsText = "Thanks again!"}
        end
    end

end

function onClientUse(self, msg)

    if self:GetVar("MGState") == 1 then
        self:PlayAnimation{ animationID = "missionState2" }
        self:DisplayChatBubble {wsText = "Why aren't you done yet?!"}
        self:SetVar("MGState", 2)

    elseif self:GetVar("MGState") == 2 then
        self:PlayAnimation{ animationID = "missionState3" }
        self:DisplayChatBubble {wsText = "You did it!"}
        self:SetVar("MGState", 3)

    elseif self:GetVar("MGState") == 3 then
        self:PlayAnimation{ animationID = "missionState4" }
        self:DisplayChatBubble {wsText = "Thanks for the help!"}
        self:SetVar("MGState", 4)

    elseif self:GetVar("MGState") == 4 then
        self:PlayAnimation{ animationID = "missionState1" }
        self:DisplayChatBubble {wsText = "I need your help!"}
        self:SetVar("MGState", 1)
    end

end