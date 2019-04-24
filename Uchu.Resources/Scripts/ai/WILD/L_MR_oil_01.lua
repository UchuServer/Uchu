require('o_mis')

function onStartup(self, msg)

    local friends = self:GetObjectsInGroup{ group = "MR_Control"}.objects

    for i = 1, table.maxn (friends) do
        friends[i]:NotifyObject{name = "traplabel", ObjIDSender = self}
    end

end

function onCollisionPhantom(self, msg)

    local target = msg.objectID
	local faction = target:GetFaction()
    
    if faction and faction.faction == 1 then
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.25, "OilStunOver",self )
        GAMEOBJ:GetTimer():AddTimerWithCancel( 2.0, "OilDone",self )

        target:AddSkill{ skillID = 170 }
        target:CastSkill{ optionalTargetID = self, skillID = 170 }
        target:SetUserCtrlCompPause{bPaused = true}
        target:PlayAnimation{animationID = "hit-racecar"}
        storeObjectByName(self, "Slicker", target)
    end

end

function onTimerDone(self, msg)

	if msg.name == "OilStunOver" then
        getObjectByName(self,"Slicker"):SetUserCtrlCompPause{bPaused = false}

	elseif msg.name == "OilDone" then
        getObjectByName(self,"Slicker"):RemoveSkill{ skillID = 170 }
        getObjectByName(self,"Slicker"):AddSkill{ skillID = 66 }
        getObjectByName(self,"Slicker"):CastSkill{ optionalTargetID = self, skillID = 66 }
    end

end 

function onUse(self)

    GAMEOBJ:DeleteObject(self)

end