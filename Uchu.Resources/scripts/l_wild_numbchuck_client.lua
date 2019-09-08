require('o_mis')
require('L_AG_NPC')

function onStartup(self,msg)

    self:SetVar("MGState", 1)
    self:SetVar("Sleeping", 1)
    self:SetProximityRadius { radius = 20 }

end

function onProximityUpdate(self, msg)

	local target = msg.objId
	local faction = target:GetFaction()

    if faction and faction.faction == 1 then
        if msg.status == "LEAVE" then
            if self:GetVar("Sleeping") == 0 then
                self:PlayAnimation{ animationID = "sleep" }
                self:SetVar("Sleeping", 2)
                self:DisplayChatBubble {wsText = "I'm going back to bed."}
                GAMEOBJ:GetTimer():AddTimerWithCancel( 7.5, "RestoreSleep",self )
            end
        end
    end

end

function onClientUse(self, msg)

    if self:GetVar("MGState") == 1 then
        if self:GetVar("Sleeping") == 1 then
            self:SetVar("Sleeping", 2)
            self:PlayAnimation{ animationID = "wake" }
            GAMEOBJ:GetTimer():AddTimerWithCancel( 7.5, "WakeState1",self )
            self:DisplayChatBubble {wsText = "Huh?"}
        elseif self:GetVar("Sleeping") == 0 then
            self:SetVar("Sleeping", 2)
            self:PlayAnimation{ animationID = "sleep" }
            GAMEOBJ:GetTimer():AddTimerWithCancel( 7.5, "RestoreSleep",self )
            self:DisplayChatBubble {wsText = "Go do that thing I asked you to do."}
            self:SetVar("MGState", 2)
        end

    elseif self:GetVar("MGState") == 2 then
        if self:GetVar("Sleeping") == 1 then
            self:SetVar("Sleeping", 2)
            self:PlayAnimation{ animationID = "wake" }
            GAMEOBJ:GetTimer():AddTimerWithCancel( 7.5, "WakeState2",self )
            self:DisplayChatBubble {wsText = "Wuh?"}
        elseif self:GetVar("Sleeping") == 0 then
            self:SetVar("Sleeping", 2)
            self:PlayAnimation{ animationID = "sleep" }
            GAMEOBJ:GetTimer():AddTimerWithCancel( 7.5, "RestoreSleep",self )
            self:DisplayChatBubble {wsText = "Come back when you're ready."}
            self:SetVar("MGState", 3)
        elseif self:GetVar("Sleeping") == 2 then
            self:SetVar("MGState", 3)
        end

    elseif self:GetVar("MGState") == 3 then
        if self:GetVar("Sleeping") == 1 then
            self:SetVar("Sleeping", 2)
            self:PlayAnimation{ animationID = "wake" }
            GAMEOBJ:GetTimer():AddTimerWithCancel( 7.5, "WakeState3",self )
            self:DisplayChatBubble {wsText = "Derp?"}
        elseif self:GetVar("Sleeping") == 0 then
            self:SetVar("Sleeping", 2)
            self:PlayAnimation{ animationID = "sleep" }
            GAMEOBJ:GetTimer():AddTimerWithCancel( 7.5, "RestoreSleep",self )
            self:DisplayChatBubble {wsText = "Thanks again!"}
            self:SetVar("MGState", 4)
        elseif self:GetVar("Sleeping") == 2 then
            self:SetVar("MGState", 4)
        end

    elseif self:GetVar("MGState") == 4 then
        if self:GetVar("Sleeping") == 1 then
            self:SetVar("Sleeping", 2)
            self:PlayAnimation{ animationID = "wake" }
            GAMEOBJ:GetTimer():AddTimerWithCancel( 7.5, "WakeState4",self )
            self:DisplayChatBubble {wsText = "Bwah?"}
        elseif self:GetVar("Sleeping") == 0 then
            self:SetVar("Sleeping", 2)
            self:PlayAnimation{ animationID = "sleep" }
            GAMEOBJ:GetTimer():AddTimerWithCancel( 7.5, "RestoreSleep",self )
            self:DisplayChatBubble {wsText = "Thanks again!"}
            self:SetVar("MGState", 1)
        elseif self:GetVar("Sleeping") == 2 then
            self:SetVar("MGState", 1)
        end
    end

end

function onTimerDone(self, msg)

	if msg.name == "WakeState1" then
        local objs = self:GetProximityObjects().objects

        if table.getn(objs) >= 2 then
            self:PlayAnimation{ animationID = "missionState1" }
            self:DisplayChatBubble {wsText = "I need your help!"}
            self:SetVar("Sleeping", 0)
        elseif table.getn(objs) == 1 then
            self:PlayAnimation{ animationID = "sleep" }
            self:DisplayChatBubble {wsText = "I'm going back to bed."}
            GAMEOBJ:GetTimer():AddTimerWithCancel( 7.5, "RestoreSleep",self )
        end

    elseif msg.name == "WakeState2" then
        local objs = self:GetProximityObjects().objects

        if table.getn(objs) >= 2 then
            self:PlayAnimation{ animationID = "missionState2" }
            self:DisplayChatBubble {wsText = "Why aren't you done yet?!"}
            self:SetVar("Sleeping", 0)
            GAMEOBJ:GetTimer():AddTimerWithCancel( 2.3, "GoBackToSleep",self )
        elseif table.getn(objs) == 1 then
            self:PlayAnimation{ animationID = "sleep" }
            self:DisplayChatBubble {wsText = "I'm going back to bed."}
            GAMEOBJ:GetTimer():AddTimerWithCancel( 7.5, "RestoreSleep",self )
        end

    elseif msg.name == "WakeState3" then
        local objs = self:GetProximityObjects().objects

        if table.getn(objs) >= 2 then
            self:PlayAnimation{ animationID = "missionState3" }
            self:DisplayChatBubble {wsText = "You did it!"}
            self:SetVar("Sleeping", 0)
            GAMEOBJ:GetTimer():AddTimerWithCancel( 7.3, "GoBackToSleep",self )
        elseif table.getn(objs) == 1 then
            self:PlayAnimation{ animationID = "sleep" }
            self:DisplayChatBubble {wsText = "I'm going back to bed."}
            GAMEOBJ:GetTimer():AddTimerWithCancel( 7.5, "RestoreSleep",self )
        end

    elseif msg.name == "WakeState4" then
        local objs = self:GetProximityObjects().objects

        if table.getn(objs) >= 2 then
            self:PlayAnimation{ animationID = "missionState4" }
            self:DisplayChatBubble {wsText = "Thanks for the help!"}
            self:SetVar("Sleeping", 0)
            GAMEOBJ:GetTimer():AddTimerWithCancel( 7.6, "GoBackToSleep",self )
        elseif table.getn(objs) == 1 then
            self:PlayAnimation{ animationID = "sleep" }
            self:DisplayChatBubble {wsText = "I'm going back to bed."}
            GAMEOBJ:GetTimer():AddTimerWithCancel( 7.5, "RestoreSleep",self )
        end

    elseif msg.name == "RestoreSleep" then
        self:SetVar("Sleeping", 1)

    elseif msg.name == "GoBackToSleep" then
        self:PlayAnimation{ animationID = "sleep" }
        self:DisplayChatBubble {wsText = "I'm going back to bed."}
        GAMEOBJ:GetTimer():AddTimerWithCancel( 7.5, "RestoreSleep",self )
    end
    
end
