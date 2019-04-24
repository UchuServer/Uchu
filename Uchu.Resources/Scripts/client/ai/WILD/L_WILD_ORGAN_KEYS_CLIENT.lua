function onStartup(self)

    self:SetVar("keyuseclient", 1)
    self:RequestPickTypeUpdate()

end

function onGetPriorityPickListType(self, msg)

    if self:GetVar("keyuseclient") == 1 then
        msg.ePickType = -1 -- Non-Interactive pick type
        return msg
    else
        msg.ePickType = 14 -- Interactive pick type
        return msg
    end

end

function onNotifyClientObject(self, msg)      

    if msg.name == "Click" then
        self:SetVar("keyuseclient", 0)
        self:RequestPickTypeUpdate()
    elseif msg.name == "NoClick" then
        self:SetVar("keyuseclient", 1)
        self:RequestPickTypeUpdate()
    end

end