--------------------------------------------------------------------
--script that tells the door to open once the switch is pressed and the module is built
--------------------------------------------------------------------

function onStartup(self)
   self:SetVar("ModuleUp", false)
   self:SetVar("IAMengaged", false)
   self:SetVar("HEISengaged", false)
end

function onNotifyObject(self, msg)
   if msg.name == "ModuleBuilt" then
      --print("module built")
      self:SetVar("ModuleUp", true)
    elseif msg.name == "pressed" then
      self:SetVar("HEISengaged", true)
   elseif msg.name == "depressed" then
      self:SetVar("HEISengaged", false)
   end
end

function onFireEvent(self, msg)
    if msg.args == "down" and self:GetVar("ModuleUp") == true then
    self:SetVar("IAMengaged", true)
    local object = self:GetObjectsInGroup{group = "SpiderPsSwitch2", ignoreSpawners = true}.objects[1]
    if object then
        object:NotifyObject{name = "pressed", ObjIDSender = self}
    end
    local group = self:GetObjectsInGroup{group = "SpiderPlatforms", ignoreSpawners = true}.objects
    for i, object in pairs(group) do
        if object then
            object:GoToWaypoint{iPathIndex = 1, bAllowPathingDirectionChange = true}
        end
    end
    elseif msg.args == "up" then
      self:SetVar("IAMengaged", false)
      local object2 = self:GetObjectsInGroup{group = "SpiderPsSwitch2", ignoreSpawners = true}.objects[1]
      if object2 then
         object2:NotifyObject{name = "depressed", ObjIDSender = self}
      end
      if self:GetVar("HEISengaged") == false then
        local group = self:GetObjectsInGroup{group = "SpiderPlatforms", ignoreSpawners = true}.objects
        for i, object in pairs(group) do
            if object then
                object:GoToWaypoint{iPathIndex = 0, bAllowPathingDirectionChange = true}
            end
        end
      end
   end
end