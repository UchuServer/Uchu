------------------------------------------------------------------------------
--paradox refinery pipe 3 quickbuild that update the mission once all are built
------------------------------------------------------------------------------





function onStartup(self)
   
   self:SetVar("Pipe1Built", false)
   self:SetVar("Pipe2Built", false)
   self:SetVar("AmBuilt", false)
   
end






function onRebuildNotifyState(self, msg)

   if msg.iState == 2 then
      
      self:SetVar("AmBuilt", true)
      
      if self:GetVar("Pipe1Built") ==true and self:GetVar("Pipe2Built") == true then
         
         print("mission completed")
         msg.player:UpdateMissionTask{taskType = "complete", value = 769, value2 = 1, target = self}
         msg.player:PlayCinematic{pathName = "ParadoxPipeFinish", leadIn = 2.0}
         return
         
      end
      
      local object = self:GetObjectsInGroup{group = "ParadoxPipes1", ignoreSpawners = true}.objects[1]
      
      if object then
         
         print("telling pipe 1 I am built")
         --object:PlayFXEffect{name = "imaginationbase", effectID = 114, effectType = "onrebuild"}
         object:NotifyObject{name = "Pipe3Up", ObjIDSender = self}
      
      end
      
      local object = self:GetObjectsInGroup{group = "ParadoxPipes2", ignoreSpawners = true}.objects[1]
      
      if object then
         
         print("telling pipe 2 I am built")
         --object:PlayFXEffect{name = "imaginationbase", effectID = 114, effectType = "onrebuild"}
         object:NotifyObject{name = "Pipe3Up", ObjIDSender = self}
      
      end
      
   elseif msg.iState == 4 then
      
      self:SetVar("AmBuilt", false)
      
      local object = self:GetObjectsInGroup{group = "ParadoxPipes1", ignoreSpawners = true}.objects[1]
      
      if object then
         
         print("telling pipe 1 I am NOT built")
         --object:PlayFXEffect{name = "imaginationbase", effectID = 114, effectType = "onrebuild"}
         object:NotifyObject{name = "Pipe3Down", ObjIDSender = self}
      
      end
      
      local object = self:GetObjectsInGroup{group = "ParadoxPipes2", ignoreSpawners = true}.objects[1]
      
      if object then
         
         print("telling pipe 2 I am NOT built")
         --object:PlayFXEffect{name = "imaginationbase", effectID = 114, effectType = "onrebuild"}
         object:NotifyObject{name = "Pipe3Down", ObjIDSender = self}
      
      end
   end
end






function onNotifyObject(self, msg)
   
   if msg.name == "Pipe1Up" then
      
      self:SetVar("Pipe1Built", true)
   
   elseif msg.name == "Pipe1Down" then
   
      self:SetVar("Pipe1Built", false)
   
   elseif msg.name == "Pipe2Up" then
   
      self:SetVar("Pipe2Built", true)
   
   elseif msg.name == "Pipe2Down" then
   
      self:SetVar("Pipe2Built", false)
   
   end
end