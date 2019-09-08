require('o_mis')

function onUse(self, msg)

   local friends = self:GetObjectsInGroup{ group = "GP_Control" }.objects
   for i = 1, table.maxn (friends) do 
      if friends[i]:GetLOT().objtemplate == 5899 then
         friends[i]:NotifyObject{name = "reset", ObjIDSender = self}

      elseif friends[i]:GetLOT().objtemplate == 5900 then
         friends[i]:NotifyObject{name = "reset", ObjIDSender = self}

      elseif friends[i]:GetLOT().objtemplate == 5901 then
         friends[i]:NotifyObject{name = "reset", ObjIDSender = self}

      elseif friends[i]:GetLOT().objtemplate == 5925 then
         friends[i]:NotifyObject{name = "reset", ObjIDSender = self}

      elseif friends[i]:GetLOT().objtemplate == 5926 then
         friends[i]:NotifyObject{name = "reset", ObjIDSender = self}

      elseif friends[i]:GetLOT().objtemplate == 5927 then
         friends[i]:NotifyObject{name = "reset", ObjIDSender = self}

      elseif friends[i]:GetLOT().objtemplate == 5855 then
         friends[i]:NotifyObject{name = "reset", ObjIDSender = self}
      end
   end

end
