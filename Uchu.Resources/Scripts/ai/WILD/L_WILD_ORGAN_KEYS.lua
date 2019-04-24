function onStartup(self)

    self:AddObjectToGroup{group = "KeysForOrgan"}

end

function onUse(self, msg)

    local parentObj = self:GetParentObj().objIDParent
    local player = msg.user

    if parentObj:ActivityUserExists{userID = player}.bExists then
        local keyname = self:GetName().name
        -- Play key and minifig animation
        player:PlayAnimation{ animationID = keyname }
        self:PlayAnimation{ animationID = "interact" }
    end

end

function onNotifyObject(self, msg)      

    if msg.name == "Clickable" then
        self:NotifyClientObject{name = "Click"}
    elseif msg.name == "Unclickable" then
        self:NotifyClientObject{name = "NoClick"}
    end
    
end