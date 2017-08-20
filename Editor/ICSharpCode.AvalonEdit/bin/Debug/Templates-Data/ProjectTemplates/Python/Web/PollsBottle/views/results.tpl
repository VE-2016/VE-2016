% rebase('layout.tpl', title='Poll Page', year=year)

<h2>{{poll.text}}</h2>
<br/>

%for choice in poll.choices:
<div class="row">
    <div class="col-sm-5">{{choice.text}}</div>
    <div class="col-sm-5">
        <div class="progress">
            <div class="progress-bar" role="progressbar" aria-valuenow="{{choice.votes}}" aria-valuemin="0" aria-valuemax="{{poll.total_votes}}" style="width: {{choice.votes_percentage}}%;">
                {{choice.votes}}
            </div>
        </div>
    </div>
</div>
%end

<br/>
<a class="btn btn-primary" href="/poll/{{poll.key}}">Vote again?</a>
