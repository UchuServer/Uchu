------------------------------------------------------------------------------
--paradox refinery pipe 2 quickbuild that update the mission once all are built
------------------------------------------------------------------------------





function onStartup(self)
   
   self:SetVar("Pipe1Built", false)
   self:SetVar("AmBuilt", false)
   self:SetVar("Pipe3Built", false)
   
end






function onRebuildNotifyState(self, msg)

   if msg.iState == 2 then
      
      self:SetVar("AmBuilt", true)
      
      if self:GetVar("Pipe1Built") ==true and self:GetVar("Pipe3Built") == true then
         
         print("mission completed")
         msg.player:UpdateMissionTask{taskType = "complete", value = 769, value2 = 1, target = self}
         msg.player:PlayCinematic{pathName = "ParadoxPipeFinish", leadIn = 2.0}
         return
         
      end
      
      local object = self:GetObjectsInGroup{group = "ParadoxPipes1", ignoreSpawners = true}.objects[1]
      
      if object then
         
         print("telling pipe 1 I am built")
         --object:PlayFXEffect{name = "imaginationbase", effectID = 114, effectType = "onrebuild"}
         object:NotifyObject{name = "Pipe2Up", ObjIDSender = self}
      
      end
      
      local object = self:GetObjectsInGroup{group = "ParadoxPipes3", ignoreSpawners = true}.objects[1]
      
      if object then
         
         print("telling pipe 3 I am built")
         --object:PlayFXEffect{name = "imaginationbase", effectID = 114, effectType = "onrebuild"}
         object:NotifyObject{name = "Pipe2Up", ObjIDSender = self}
      
      end
      
   elseif msg.iState == 4 then
      
      self:SetVar("AmBuilt", false)
      
      local object = self:GetObjectsInGroup{group = "ParadoxPipes1", ignoreSpawners = true}.objects[1]
      
      if object then
         
         print("telling pipe 1 I am NOT built")
         --object:PlayFXEffect{name = "imaginationbase", effectID = 114, effectType = "onrebuild"}
         object:NotifyObject{name = "Pipe2Down", ObjIDSender = self}
      
      end
      
      local object = self:GetObjectsInGroup{group = "ParadoxPipes3", ignoreSpawners = true}.objects[1]
      
      if object then
         
         print("telling pipe 3 I am NOT built")
         --object:PlayFXEffect{name = "imaginationbase", effectID = 114, effectType = "onrebuild"}
         object:NotifyObject{name = "Pipe2Down", ObjIDSender = self}
      
      end
   end
end






function onNotifyObject(self, msg)
   
   if msg.name == "Pipe1Up" then
      
      self:SetVar("Pipe1Built", true)
   
   elseif msg.name == "Pipe1Down" then
   
      self:SetVar("Pipe1Built", false)
   
   elseif msg.name == "Pipe3Up" then
   
      self:SetVar("Pipe3Built", true)
   
   elseif msg.name == "Pipe3Down" then
   
      self:SetVar("Pipe3Built", false)
   
   end
end