------------------------------------------------------
--script telling the module when when the pipe is repaired
------------------------------------------------------

function onRebuildNotifyState(self, msg)
   if msg.iState == 2 then
      --print("pipe fixed")
      local object = self:GetObjectsInGroup{group = "QBModule1", ignoreSpawners = true}.objects[1]
      if object then
         object:NotifyObject{name = "PipeFixed", ObjIDSender = self}
      end
      --[[local object = self:GetObjectsInGroup{group = "brokenPipe1", ignoreSpawners = true}.objects[1]
      if object then
         object:StopFXEffect{name = "maelstromobject"}
      end--]]
   end
end