------------------------------------------------------------------------
--Pirate Side Scroller Zone Script
------------------------------------------------------------------------



function onMessageBoxRespond(self, msg)
   
   local player = msg.sender

   if(msg.iButton == 1 and msg.identifier == "ActivityButton") then
      
      player:DisplayMessageBox{bShow = true, imageID = 1, callbackClient = self, text = "Do you want to return to Gnarled Forest?" , identifier = "Exit"}
      
   elseif(msg.iButton == 1 and msg.identifier == "Exit") then
      
      player:TransferToLastNonInstance()
      
   end
end