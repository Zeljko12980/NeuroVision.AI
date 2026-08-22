import { useEffect, useState } from "react";

export const DEFAULT_USER_AVATAR = "/images/user/default-user.svg";

type ProfileAvatarProps = {
    src?: string | null;
    alt?: string;
    className?: string;
};

export default function ProfileAvatar({
    src,
    alt = "User",
    className,
}: ProfileAvatarProps) {
    const [failed, setFailed] = useState(false);
    const imageSrc = !src || failed ? DEFAULT_USER_AVATAR : src;

    useEffect(() => {
        setFailed(false);
    }, [src]);

    return (
        <img
            src={imageSrc}
            alt={alt}
            className={className}
            onError={() => setFailed(true)}
        />
    );
}
