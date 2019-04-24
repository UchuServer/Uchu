function onFireEventServerSide(self, msg)
    if msg.args == "removeBricks" then
        msg.senderID:ClearInventory{which = "brick"}
    end
end 