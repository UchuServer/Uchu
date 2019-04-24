------------------------------------------------
--switch to fire the cannon in the second room
------------------------------------------------

--local INTERACT_RADIUS = 1.5

function onStartup(self)
   self:AddObjectToGroup{group = "spiderDrill"}
   --self:SetProximityRadius{radius = INTERACT_RADIUS}
   self:SetVar("drillInPlace", false)
   self:SetVar("rebuildComplete", false)
   self:SetVar("FireTime", 3)
   --print("starting up the cannon switch!")
end

function onNotifyObject(self, msg)
   --print("switch got a message ")
   if msg.name == "drillDown" then
      self:SetVar("drillInPlace", true)
      --print("drill in place")
   elseif msg.name == "drillUp" then
      self:SetVar("drillInPlace", false)
      --print("drill up")
    elseif msg.name == "readyToMove" then
        self:SetVar("rebuildComplete", true)
        --print("ready to move the drill")
    elseif msg.name == "notReadyToMove" then
        self:SetVar("rebuildComplete", false)
   end
end

function onFireEvent(self, msg)
   --print("switch activated!!")
   --local isHuman = msg.objId:GetIsHumanPlayer().bIsHuman
   --print(tostring(isHuman))
    local friends = self:GetObjectsInGroup{group = "spiderDrill", ignoreSpawner = true}.objects
    for i, object in pairs(friends) do
        if (object:GetLOT().objtemplate == 6528) then
            --print("telling drill to kill the spider")
            object:PlayFXEffect{name = "moviespotlight", effectID = 503, effectType = "create"}
        end
    end
    GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("FireTime")  , "FireTime", self )
    if (self:GetVar("rebuildComplete") == true) and (self:GetVar("drillInPlace") == true) then
    self:NotifyClientObject{name = "playCine"}
      --print("ready to fire, searching for trigger")
        local friends = self:GetObjectsInGroup{group = "spiderDrill", ignoreSpawner = true}.objects
        for i, object in pairs(friends) do
            if (object:GetLOT().objtemplate == 6457) then
                --print("telling drill to kill the spider")
                object:NotifyObject{name = "shocked", ObjIDSender = self}   
            elseif (object:GetLOT().objtemplate == 5651) then
                object:ToggleTrigger{enable = false}
            end
    --[[elseif (self:GetVar("rebuildComplete") == true) and (self:GetVar("drillInPlace") == false) then
        local friends = self:GetObjectsInGroup{group = "spiderDrill", ignoreSpawner = true}.objects
        for i, object in pairs(friends) do
            if (object:GetLOT().objtemplate == 6528) then
                object:NotifyObject{name = "missTheSpider", ObjIDSender = self}
            end--]]
        end
   end
end

function onTimerDone(self, msg)
    local friends = self:GetObjectsInGroup{group = "spiderDrill", ignoreSpawner = true}.objects
    for i, object in pairs(friends) do
        if (object:GetLOT().objtemplate == 6528) then
            object:StopFXEffect{name = "moviespotlight"}
        end
    end
end