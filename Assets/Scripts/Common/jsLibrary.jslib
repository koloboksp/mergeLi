mergeInto(LibraryManager.library, {
    loadData: function(yourkey){
        var returnStr = "";

        if(localStorage.getItem(UTF8ToString(yourkey)) !==null)
        {
            returnStr = localStorage.getItem(UTF8ToString(yourkey));
        }

        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    },
    saveData: function(yourkey, yourdata){
        localStorage.setItem(UTF8ToString(yourkey), UTF8ToString(yourdata));
    },
    deleteKey: function(yourkey){
        localStorage.removeItem(UTF8ToString(yourkey));
    },
    deleteAllKeys: function(prefix){
        prefix = UTF8ToString(prefix);
        
        for ( var i = 0, len = localStorage.length; i < len; ++i ) {
            var key = localStorage.key(i);
            if(key != null && key.startsWith(prefix)){
                localStorage.removeItem(key);
            }
        }
    },
    listAllKeys: function(prefix){
        var keyList = [];
        console.log("Local Storage Length:", localStorage.length);
        prefix = UTF8ToString(prefix);
        console.log("Prefix", prefix);
        for ( var i = 0, len = localStorage.length; i < len; ++i ) {
            var key = localStorage.key(i);
            console.log("Prefixed Key:", key);
            var hasPrefix = key.startsWith(prefix);
            console.log("Has Prefix of: ", prefix, ": ", hasPrefix);
            if(key != null && hasPrefix){
                key = key.substring(prefix.length);
                console.log("    Key:", key);
                keyList.push(key);
            }
        }
        console.log("KeyList Length:", keyList.length);
        console.log("    KeyList in JS:", keyList);
        var result = keyList.join(',');
        console.log("    KeyList in JS after join:", result);
        var bufferSize = lengthBytesUTF8(result) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(result, buffer, bufferSize);
        return buffer;
    }
});