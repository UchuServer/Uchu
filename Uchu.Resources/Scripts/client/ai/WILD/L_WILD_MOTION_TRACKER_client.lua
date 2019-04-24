function onNotifyClientObject(self, msg)

    if msg.name == "Lostya" then
        self:DisplayChatBubble {wsText = "Where are you?"}
        --print "Where are you client?!"
    end

    if msg.name == "Gotcha" then
        --print "I see you client!"
        self:DisplayChatBubble {wsText = "I see you!"}
    end

end