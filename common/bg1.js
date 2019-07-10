function SetBag() {
    document.getElementsByClassName('panel')[0].style.backgroundColor = '#b0b0b0';
    // b.指定消息背景
    $('.chat_item.top').css('background', '#b0b0b0');

    // 2. a.列表的边
    var arr = document.getElementsByClassName('chat_item');
    for (var i = 0; i < arr.length; i++)
    {
        arr[i].style.borderBottomColor = '#808080';
    };
    $('.contact_list .contact_title').css('borderBottomColor', '#FFF');

    //  b  .contact_list.contact_item
    $('.contact_list .contact_item').css('borderBottomColor', '#808080');

    //3.分组标题栏
    $('.contact_list .contact_title').css('background', '#808080');
    $('.contact_list .contact_title').css('color', '#FFF');

    //4.  搜索框
    $('.search_bar .frm_search').css('background', '#808080');

    //5. 选中底色 .chat_item.active
    //"$('.chat_item.active').css('background','#6AAEFF');" +
    //"$('.chat_item.inactive').css('background','#808080');" +

    //6. .tab .tab_item:after    tab标题的右边框设置
    var style = document.createElement('style');
    var text = document.createTextNode('.tab:after{content:none;border-bottom:1px solid red;}');
    style.appendChild(text);
    document.body.appendChild(style);
    $('.tab').addClass('.tab:after');

    //7. .tab_item  的边框颜色
    var style1 = document.createElement('style');
    var text1 = document.createTextNode('.tab_item:after{content:none;}');
    style.appendChild(text1);
    document.body.appendChild(style1);
    $('.tab_item').addClass('.tab:after');
}
