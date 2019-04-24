-----------------------------------------------------------------
--Spider Boss 2 script
-----------------------------------------------------------------

function onStartup(self)
    
    local group = self:GetObjectsInGroup{group = "BossInteracts", ignoreSpawners = true}.objects
   
    for i, object in pairs(group) do
   
        if object then
            --spider doesn't hate the coils here--
            self:AddThreatRating{newThreatObjID = object, ThreatToAdd = -1000}
        end
    end
end