function onStartup(self)  
   
   self:AddObjectToGroup{group = "spiderBoulder"}
   --self:SetVar("killself", false)

end


--------------------------------------------------------------------------------
--tells the leg to get trapped inside the bear trap if built
--------------------------------------------------------------------------------

function onNotifyObject(self, msg)
   
   if msg.name == "crash" then
      --self:SetVar("killself", true)
      self:Die()
   end
end